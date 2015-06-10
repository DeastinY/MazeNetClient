import os
from flask import Flask, request, render_template
from werkzeug import secure_filename

UPLOAD_FOLDER = '/uploads/'
ALLOWED_EXTENSIONS = set('exe')

app = Flask(__name__)
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER


def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1] in ALLOWED_EXTENSIONS


@app.route('/')
def main():
    return render_template('index.html')


@app.route('/home')
def home():
    return render_template('index.html')


@app.route('/upload')
def upload():
    return render_template('upload.html')


@app.route('/upload', methods=['POST'])
def upload_file():
    print(request.get_data())
    file = request.files['file']
    if file and allowed_file(file.filename):
        filename = secure_filename(file.filename)
        file.save(os.path.join(app.config[UPLOAD_FOLDER], filename))
        return redirect(url_for('uploaded_file, filename=filename'))
    return render_template('index.html')

if __name__ == '__main__':
    app.run(debug=True)
