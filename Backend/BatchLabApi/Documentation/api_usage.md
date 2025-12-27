# Heading 1 
This documents descibres how consumers can use the api for retreaving and posting data.

- **GET BY ID** 
    verb: GET
    endpoint: /jobs/{id}
    params:
    - {id} *guuid string id*
    returns: {
        "Id": "GUIID"
        "Description": "JOB_DESCRIPTION"
        "Status": "JOB_STATUS"
        "CreatedAt": "CREATED_AT_DATE"
    }

- **GET ALL**
    verb: GET
    endpoint: /jobs/
    returns: [{
        "Id": "GUIID"
        "Description": "JOB_DESCRIPTION"
        "Status": "JOB_STATUS"
        "CreatedAt": "CREATED_AT_DATE"
    }]
- **POST**
    verb: POST
    endpoint: /jobs/
    body: {
        "Description": "JOB_DESCRIPTION"
    }
    returns: {
        "Id": "GUIID"
        "Description": "JOB_DESCRIPTION"
        "Status": "JOB_STATUS"
        "CreatedAt": "CREATED_AT_DATE"
    }