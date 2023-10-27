import dlib
import numpy as np
import sqlite3
from PIL import Image
import sys

# Загружаем модели детектора лиц и распознавания
detector = dlib.get_frontal_face_detector()
sp = dlib.shape_predictor('Scripts/dlib/shape_predictor_68_face_landmarks.dat')
facerec = dlib.face_recognition_model_v1('Scripts/dlib/dlib_face_recognition_resnet_model_v1.dat')

# Открываем соединение с базой данных
conn = sqlite3.connect('face_recognition.db')
c = conn.cursor()

# Создаём таблицы, если их ещё нет
c.execute("""
CREATE TABLE IF NOT EXISTS People (
    id INTEGER PRIMARY KEY,
    descriptor BLOB,
    first_name TEXT,
    middle_name TEXT,
    last_name TEXT,
    phone_number TEXT)
""")

c.execute("""CREATE TABLE IF NOT EXISTS UserImages (
                id INTEGER PRIMARY KEY,
                image_id INTEGER,
                user_id INTEGER,
                FOREIGN KEY(image_id) REFERENCES Images(Id),
                FOREIGN KEY(user_id) REFERENCES People(id))""")

conn.commit()

# Загружаем изображение
image_path = sys.argv[1] # TODO: замените на путь к вашему изображению
path = sys.argv[2]
img = dlib.load_rgb_image(image_path)

# Обнаруживаем лицо и вычисляем дескриптор
dets = detector(img, 1)

for k, d in enumerate(dets):
    shape = sp(img, d)
    face_descriptor = facerec.compute_face_descriptor(img, shape)

    # Сравниваем с лицами в базе данных
    c.execute("SELECT * FROM people")
    rows = c.fetchall()
    match = False
    for row in rows:
        stored_descriptor = np.frombuffer(row[1], dtype=np.float64)
        dist = np.linalg.norm(face_descriptor - stored_descriptor)

        if dist < 0.6:  # TODO: подберите подходящий порог расстояния
            match = True
            break
            
    user_id = row[0] if match else None
    # Если совпадений не найдено, сохраняем лицо в базу данных
    if not match:
        db_face_descriptor = np.array(face_descriptor).tobytes()
        c.execute("INSERT INTO people (descriptor) VALUES (?)", (db_face_descriptor,))
        # Получаем ID добавленного пользователя
        user_id = c.lastrowid

    # Привязываем лицо к фотографии (существующему или новому пользователю)
    c.execute("SELECT Id FROM Images WHERE path = ?", (path,))
    result = c.fetchone()
    image_id = result[0]
    c.execute("INSERT INTO UserImages (ImageId, UserId) VALUES (?, ?)", (image_id, user_id))

conn.commit()
conn.close()
