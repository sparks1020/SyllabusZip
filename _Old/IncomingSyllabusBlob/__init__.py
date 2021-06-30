import logging
import azure.functions as func
from azure.cognitiveservices.vision.computervision import ComputerVisionClient
from azure.cognitiveservices.vision.computervision.models import OperationStatusCodes, ReadOperationResult, AnalyzeResults, ReadResult, TextRecognitionResultDimensionUnit
from msrest.authentication import CognitiveServicesCredentials
from PIL import Image
import os
import json
import time
import io

def main(myblob: func.InputStream, doc: func.Out[func.Document]):
    logging.info(f"Python blob trigger function processed blob \n"
                 f"Name: {myblob.name}\n"
                 f"Blob Size: {myblob.length} bytes")
    get_json = {}
    get_json['text'], get_json['bounding_boxes'] = read_results(myblob)
    
    doc.set(func.Document.from_json(json.dumps(get_json)))


def read_results(myblob: io.BufferedIOBase):
    # Read the image file
    image_stream = myblob

    cog_key = 'REDACTED'
    cog_endpoint = 'https://eastus.api.cognitive.microsoft.com/'

    # Get a client for the computer vision service
    computervision_client = ComputerVisionClient(cog_endpoint, CognitiveServicesCredentials(cog_key))

    # Use the Computer Vision service to find text in the image
    #read_results = computervision_client.recognize_printed_text_in_stream(image_stream)
    rawHttpResponse = computervision_client.read_in_stream(image_stream, 'en', None, True)

    ## The Read API is asynchronous, so we have to poll until it's done...
    # Get ID from returned headers
    operationLocation = rawHttpResponse.headers["Operation-Location"]
    idLocation = len(operationLocation) - 36 # 36 is the length of the operation ID (a GUID w dashes)
    operationId = operationLocation[idLocation:]

    # SDK call
    result: ReadOperationResult = None
    while True:
        result = computervision_client.get_read_result(operationId)
        if result.status not in [OperationStatusCodes.not_started, OperationStatusCodes.running]:
            break
        time.sleep(1)

    analyze_result: AnalyzeResults = result.analyze_result
    read_results: [ReadResult] = analyze_result.read_results

    # Process the text line by line
    text_results = ''
    text_size = []
    for page in read_results:
        for line in page.lines:

            # Show the position of the line of text
            #l,t,w,h = list(map(int, line.bounding_box.split(',')))

            # Read the words in the line of text
            line_text = ''
            for word in line.words:
                line_text += word.text + ' '
                
            #print(line_text.rstrip())
            #print(line.bounding_box)
            bounding_box = map(lambda x: int(round(x * 300)), torect(line.bounding_box)) if page.unit == TextRecognitionResultDimensionUnit.inch else torect(line.bounding_box)
            text_info = {
                'page': page.page,
                'line': line_text.rstrip(),
                'line_height': line_text.isupper(),
                'bounding_box': ",".join(map(str, bounding_box)),
                'rect': tuple(bounding_box), #(l, t, w, h)
            }
            text_size.append(text_info)
            text_results += line_text + '\n'

    #outputting data to json object
    json_results = json.dumps(text_results)

    return json_results, text_size

def torect(dim):
    # l t w h
    return (dim[0], dim[1], dim[4]-dim[0], dim[5]-dim[1])
