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

    headers = {
        'Content-Type': 'application/x-www-form-urlencoded',
        'Referer': os.getenv("CS10ServerSearch")
    }

    response = session.post(url,    data=data, headers=headers, timeout=5)
    print(response.cookies)


if __name__ == "__main__":
    get_cookie()

    # as docs said data should be encoded as application/x-www-form-urlencoded
    # as internet says i just need to send it as a dictionary. However it's not working
    # request_body = {
    #         "client_id": f"{self.client_ID}",
    #         "grant_type": "authorization_code",
    #         "code": f"{auth_code}",
    #         "redirect_uri": f"{self.redirect_uri}",
    #         "code_verifier": f"{code_verifier}"
    # }

    # response = requests.post(endpoint, data=request_body)
    # print(response)

    # headers = {'Content-Type': 'application/x-www-form-urlencoded'}
    # response = requests.post(endpoint, data=request_body, headers=headers)
    # print(response)
