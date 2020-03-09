
import json 
from keras.preprocessing.image import load_img
from keras.preprocessing.image import img_to_array
from keras.applications.vgg19 import preprocess_input
from keras.applications.vgg19 import decode_predictions
from keras.applications.vgg19 import VGG19
import numpy as np
import io
from PIL import Image
import json
from numpy import array

def run(image_bytes):

    image_bytes = json.loads(image_bytes)['data'][0]
    image_bytes = image_bytes.encode('utf-8')
    encode_len = len(image_bytes)
    print(encode_len)
    
    image = Image.frombytes('RGBA', (1315,640), image_bytes, 'raw')
    
    image = image.resize((224,224),Image.ANTIALIAS)    
    image = img_to_array(image)
    image = image[:,:,0:3]
    print(image.shape)
    image = image.reshape((1, image.shape[0], image.shape[1], image.shape[2]))
    image = preprocess_input(image)
    
    model = VGG19()
    yhat = model.predict(image)
    label = decode_predictions(yhat)
    label = label[0][0]
    return label
