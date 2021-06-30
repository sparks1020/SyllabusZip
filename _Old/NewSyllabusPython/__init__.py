import logging
import azure.functions as func
from azure.cognitiveservices.vision.computervision import ComputerVisionClient
from msrest.authentication import CognitiveServicesCredentials
import matplotlib.pyplot as plt
from PIL import Image, ImageDraw
import os
import json


def main(req: func.HttpRequest, doc: func.Out[func.Document]) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    
    get_json = read_results()

    doc.set(func.Document.from_json(request_body(get_json)))

    if name:
        return func.HttpResponse(f"Hello {name}!")
    else:
        return func.HttpResponse(
             "Please pass a name on the query string or in the request body",
             status_code=400
        )

def read_results():
    # Read the image file
    image_path = os.path.join('Images', '0001.jpg')
    image_stream = open(image_path, "rb")

    cog_key = 'REDACTED'
    cog_endpoint = 'https://eastus.api.cognitive.microsoft.com/'

    # Get a client for the computer vision service
    computervision_client = ComputerVisionClient(cog_endpoint, CognitiveServicesCredentials(cog_key))

    # Open image to display it.
    fig = plt.figure(figsize=(7, 7))
    img = Image.open(image_path)
    draw = ImageDraw.Draw(img)

    # Use the Computer Vision service to find text in the image
    read_results = computervision_client.recognize_printed_text_in_stream(image_stream)

    # Process the text line by line
    text_results = ''
    for region in read_results.regions:
        for line in region.lines:

            # Show the position of the line of text
            l,t,w,h = list(map(int, line.bounding_box.split(',')))
            draw.rectangle(((l,t), (l+w, t+h)), outline='magenta', width=5)

            # Read the words in the line of text
            line_text = ''
            for word in line.words:
                line_text += word.text + ' '
                
            print(line_text.rstrip())
            print(line.bounding_box)
            text_results += line_text + '\n'

    #outputting data to json object
    json_results = json.dump(text_results)

    return json_results
