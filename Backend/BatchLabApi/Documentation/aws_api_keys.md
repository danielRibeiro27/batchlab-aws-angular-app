# Heading 1
This provides information on how to setup the api keys necessary for the AWS SQS queue to work.

# Heading 2
Use these ambient variables for storing the api keys
    `
    $BATCHLAB_API_AWS_ACCESS_KEY_ID 'api key id with write permission
    $BATCHLAB_API_AWS_SECRET_ACCESS_KEY 'api secret key with write permission
    $BATCHLAB_WORKER_AWS_ACCESS_KEY_ID 'api key id with read permission
    $BATCHLAB_WORKER_AWS_SECRET_ACCESS_KEY 'api secret key with read permission
    $AWS_REGION 'queue region, sa-east-1
    `

# Heading 2
You can use the folowwing commands for running on a ambient that has the keys
    `
    --run api
    export AWS_ACCESS_KEY_ID=$BATCHLAB_API_AWS_ACCESS_KEY_ID
    export AWS_SECRET_ACCESS_KEY=$BATCHLAB_API_AWS_SECRET_ACCESS_KEY

    dotnet run --project BatchLabApi/BatchLabApi.csproj

    --run worker
    export AWS_ACCESS_KEY_ID=$BATCHLAB_WORKER_AWS_ACCESS_KEY_ID
    export AWS_SECRET_ACCESS_KEY=$BATCHLAB_WORKER_AWS_SECRET_ACCESS_KEY

    dotnet run --project BatchLabWorker/BatchLabWorker.csproj
    `