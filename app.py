from dotenv import load_dotenv
import os
import requests

load_dotenv()
session = requests.session()

postData = "func=ll.login"
url = os.getenv("CS10ServerSearch") + os.getenv("CS10ServerExecutable")


def get_cookie():
    data = {
        "func": "ll.login",
        "Username": os.getenv('CS10ServerUser'),
        "Password": os.getenv('CS10ServerPassword')
    }

    response = session.post(url, data=data, timeout=5)
    print(response.cookies)


if __name__ == "__main__":
    get_cookie()

