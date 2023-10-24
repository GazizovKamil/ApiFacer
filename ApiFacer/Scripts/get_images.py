import dlib
import numpy as np
import sqlite3
import pickle
import sys
import json

detector = dlib.get_frontal_face_detector()

sp = dlib.shape_predictor('Scripts/dlib/shape_predictor_68_face_landmarks.dat')
facerec = dlib.face_recognition_model_v1('Scripts/dlib/dlib_face_recognition_resnet_model_v1.dat')


conn = sqlite3.connect('face_recognition.db')
c = conn.cursor()

image_path = sys.argv[1]
path = sys.argv[2]
img = dlib.load_rgb_image(image_path)

dets = detector(img, 1)
matched_images = []
for k, d in enumerate(dets):
    shape = sp(img, d)
    face_descriptor = facerec.compute_face_descriptor(img, shape)
    c.execute("SELECT * FROM people")
    rows = c.fetchall()
    if len(rows) == 0:
        sys.exit()
    match = False
    for row in rows:
        stored_descriptor = np.frombuffer(row[1], dtype=np.float64)
        dist = np.linalg.norm(face_descriptor - stored_descriptor)
        if dist < 0.6: 
            match = True
            c.execute("SELECT image_path FROM faces WHERE user_id=?", (row[0],))
            matched_images.extend([x[0] for x in c.fetchall()])
            break
    user_id = row[0] if match else None
    if not match:
        db_face_descriptor = np.array(face_descriptor).tobytes()
        c.execute("INSERT INTO people (descriptor) VALUES (?)", (db_face_descriptor,))
        user_id = c.lastrowid
    c.execute("INSERT INTO faces (image_path, user_id) VALUES (?, ?)", (path, user_id))

conn.commit()
conn.close()

# Save matched images to a file
output_string = json.dumps(matched_images)

print(output_string)