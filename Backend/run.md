--ambient variables
$AWS_ACESS_KEY_ID 'queue key id
$AWS_SECRET_ACCESS_KEY 'queue secret key
$AWS_REGION 'queue region, sa-east-1

--run api
export AWS_ACCESS_KEY_ID=$BATCHLAB_API_AWS_ACCESS_KEY_ID
export AWS_SECRET_ACCESS_KEY=$BATCHLAB_API_AWS_SECRET_ACCESS_KEY

dotnet run --project BatchLabApi/BatchLabApi.csproj

--run worker
export AWS_ACCESS_KEY_ID=$BATCHLAB_WORKER_AWS_ACCESS_KEY_ID
export AWS_SECRET_ACCESS_KEY=$BATCHLAB_WORKER_AWS_SECRET_ACCESS_KEY

dotnet run --project BatchLabWorker/BatchLabWorker.csproj

-- run both
(export AWS_ACCESS_KEY_ID=$BATCHLAB_API_AWS_ACCESS_KEY_ID AWS_SECRET_ACCESS_KEY=$BATCHLAB_API_AWS_SECRET_ACCESS_KEY && dotnet run --project BatchLabApi/BatchLabApi.csproj) && (export AWS_ACCESS_KEY_ID=$BATCHLAB_WORKER_AWS_ACCESS_KEY_ID AWS_SECRET_ACCESS_KEY=$BATCHLAB_WORKER_AWS_SECRET_ACCESS_KEY && dotnet run --project BatchLabWorker/BatchLabWorker.csproj)
