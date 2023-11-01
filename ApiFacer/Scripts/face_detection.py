import dlib
import numpy as np
import sqlite3
import sys

# Load face detector and recognition models
detector = dlib.get_frontal_face_detector()
sp = dlib.shape_predictor('Scripts/dlib/shape_predictor_68_face_landmarks.dat')
facerec = dlib.face_recognition_model_v1('Scripts/dlib/dlib_face_recognition_resnet_model_v1.dat')

def insert_descriptor(face_descriptor, path):
    # Use a context manager to ensure the connection is closed properly
    with sqlite3.connect('face_recognition.db', timeout=20) as conn:
        c = conn.cursor()
        c.execute("PRAGMA journal_mode=WAL")  # Use Write-Ahead Logging for concurrency
        c.execute("INSERT INTO People (descriptor) VALUES (?)", (face_descriptor,))
        user_id = c.lastrowid

        # Link the face to the photograph
        c.execute("SELECT Id FROM Images WHERE path = ?", (path,))
        result = c.fetchone()
        if result:
            image_id = result[0]
            c.execute("INSERT INTO UserImages (ImageId, UserId) VALUES (?, ?)", (image_id, user_id))

        conn.commit()

# Load the image
image_path = sys.argv[1]
path = sys.argv[2]
img = dlib.load_rgb_image(image_path)

# Detect faces and compute descriptors
dets = detector(img, 1)
for k, d in enumerate(dets):
    shape = sp(img, d)
    face_descriptor = np.array(facerec.compute_face_descriptor(img, shape)).tobytes()
    insert_descriptor(face_descriptor, path)