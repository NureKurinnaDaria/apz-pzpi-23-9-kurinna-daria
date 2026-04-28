using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Створення клієнта S3 та утиліти для завантаження файлів
var s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
var transferUtility = new TransferUtility(s3Client);

static async Task UploadFileToS3Async(
    string filePath, string bucketName, string objectKey)
{
    // Асинхронне завантаження файлу до вказаного бакету
    await transferUtility.UploadAsync(filePath, bucketName, objectKey);
    // Підтвердження успішного завантаження
    Console.WriteLine(
        $"File {filePath} uploaded to s3://{bucketName}/{objectKey}");
}

// Виклик функції: завантажуємо report.pdf до бакету my-bucket
await UploadFileToS3Async("report.pdf", "my-bucket", "reports/report.pdf");

public class Function
{
    // Клієнт DynamoDB для збереження метаданих файлів
    private readonly IAmazonDynamoDB _dynamoDb;

    public Function() =>
        _dynamoDb = new AmazonDynamoDBClient();

    // Точка входу Lambda-функції: викликається при події завантаження в S3
    public async Task<string> FunctionHandler(
        S3Event s3Event, ILambdaContext context)
    {
        foreach (var record in s3Event.Records)
        {
            // Формування запиту із метаданими файлу для запису до DynamoDB
            var request = new PutItemRequest
            {
                TableName = "FileMetadata",
                Item = new Dictionary<string, AttributeValue>
                {
                    ["FileKey"]    = new() { S = record.S3.Object.Key },
                    ["Bucket"]     = new() { S = record.S3.Bucket.Name },
                    ["Size"]       = new() { N = record.S3.Object.Size.ToString() },
                    // Час завантаження у форматі ISO 8601
                    ["UploadedAt"] = new() { S = DateTime.UtcNow.ToString("o") }
                }
            };
            await _dynamoDb.PutItemAsync(request);
        }
        return "Metadata saved.";
    }
}
