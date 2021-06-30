import azure.functions as func
from importlib import import_module as use

# import the SUT
sut = use('__init__')

print(sut.read_results(open("example-syllabus.pdf", 'rb')))