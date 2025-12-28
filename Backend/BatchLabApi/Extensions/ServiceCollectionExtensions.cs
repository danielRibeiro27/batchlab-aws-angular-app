using Amazon;
using Amazon.DynamoDBv2;
using BatchLabApi.Infrastructure.Implementation;
using BatchLabApi.Infrastructure.Interface;
using BatchLabApi.Service.Implementation;
using BatchLabApi.Service.Interface;

namespace BatchLabApi.Extensions
{ 
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddJobApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            return services;
        }

        public static IServiceCollection AddJobApplicationInfrastructures(this IServiceCollection services)
        {
            services.AddSingleton<AmazonDynamoDBClient>();
            services.AddScoped<IMessageBus, SQSMessageBus>();
            services.AddScoped<IJobsRepository, DynamoDBRepository>();
            //services.AddScoped<IJobsRepository, JsonFileRepository>(provider => 
            //     new JsonFileRepository("jobs.json"));
            return services;
        }
    }
}