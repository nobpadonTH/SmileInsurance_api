using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SmilesInsurance_api.Configurations;
using SmilesInsurance_api.HostedServices;
using SmilesInsurance_api.Services.Auth;
using SmilesInsurance_api.Services.SmilesInsurance.AMLO;
using SmilesInsurance_api.Startups;
using System.Collections.Generic;

namespace SmilesInsurance_api
{
    public static class ProjectSetup
    {
        /*
            README เมื่อสร้าง Project ครั้งแรก โปรดทำตามขั้นตอนต่างๆ ดังนี้

            [ ] 1. Reverse Engineer ข้อมูลจาก Database ด้วย EF Core Powertool

            [ ] 2. เมื่อเขียน Service ให้นำเข้า Dependency Injection

            [ ] 3. ทำ AutoMapper ที่ AutoMapperProfile.cs

            [ ] 4. เมื่อเขียน Client ด้วย RestSharp ให้นำเข้า Dependency Injection

            [ ] 5. เมื่อต้องการใช้การ Log in ด้วย OAuth
                    - สร้าง Google Sheet ของ Project ที่ https://bit.ly/3yUJOYq
                        เมื่อ Copy File ไปใส่ใน Google Drive ของตัวเองแล้ว ส่งลิงค์ให้ พี่มาร์ค กับ วรรณด้วย
                    - ให้เริ่มตั้งค่าที่ → https://demoauthserver.devsiamsmile.com/
                    - ให้ตั้งค่าที่ appsettings.json - OAuth
                        - EnableOAuth → true
                        - Audience → ชื่อ Api "demoapi"
                        - Scopes → ชื่อ Api "demoapi"
                    - ตั้งค่า Permission.cs ตามใน Sheet

            [ ] 6. เมื่อต้องการใช้ Quartz
                    - ให้ตั้งค่าที่ appsettings.json - Quartz
                        - EnableQuartz → true
                        - ใน Jobs เพิ่ม Key ด้วยชื่องาน Value เป็น Cronjob Schedule

            [ ] 7. เมื่อต้องการใช้ RabbitMQ
                    - ให้ตั้งค่าที่ appsettings.json - Masstransit
                        - EnableRabbitMQ → true
                    - เมื่อขึ้น Server : devsiamsmile.com ให้ตั้งค่า
                        - Vhost → develop
                        - Username → develop
                        - Password → ***********

        */

        /// <summary>
        /// ใส่ Dependency Injection ที่ใช้ใน Project
        /// </summary>
        public static IServiceCollection ConfigDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // ใช้สำหรับข้อมูล Login
            services.AddScoped<ILoginDetailServices, LoginDetailServices>();
            services.AddScoped<IAMLOServices, AMLOServices>();

            // TODO: เมื่อเขียน Service และ Interface ของ Service ให้ใส่ที่นี้
            // services.AddSingleton // ใช้เมื่อใช้ Instance เดียวทั้ง Project
            // services.AddScoped // ใช้เมื่อแยก Instance ตาม User
            // services.AddTransient // ใช้เมื่อสร้าง Instance ใหม่ทุกครั้งที่เรียกใช้

            // TODO: ตัวอย่างการเขียน RestSharp หากไม่ใช้ให้ลบ Folder Examples ทิ้ง
            // วิธีการเขียน RestSharp
            // https://github.com/SiamsmileDev/DevKnowledgeBase/blob/develop/Example%20Code/CSharp/RestSharp%20Example.md

            // services.AddSingleton<ShortLinkClient>();
            // services.AddSingleton<SendSmsClient>();

            return services;
        }

        /// <summary>
        /// ใส่ Job Schedule ที่ใช้ใน Project
        /// </summary>
        public static IServiceCollectionQuartzConfigurator ConfigQuartz(this IServiceCollectionQuartzConfigurator q, QuartzSetting quartzSetting)
        {
            // TODO: เมื่อเขียน Job Schedule แล้วให้ใส่งานที่นี้ ให้เพิ่ม Schedule ใน appsetting.json ด้วย
            // การสร้าง Job Schedule ดูได้ที่
            // https://github.com/SiamsmileDev/DevKnowledgeBase/blob/develop/Example%20Code/CSharp/Quartz%20Job%20Scheduling.md
            // การตั้งค่า appsetting.json ดูได้ที่ https://www.freeformatter.com/cron-expression-generator-quartz.html

            // Job สำหรับการลบ Log ใน Database
            q.AddJobAndTrigger<LoggerRetentionJob>(quartzSetting);

            return q;
        }

        /// <summary>
        /// ใส่ Consumer RabbitMQ ที่ใช้ใน Project
        /// </summary>
        public static IServiceCollectionBusConfigurator ConfigRabbitMQ(this IServiceCollectionBusConfigurator configure)
        {
            // TODO: ใส่ Consumer ของ RabbitMQ ที่นี่
            // การสร้าง Consumer ใน RabbitMQ ดูได้ที่
            // configure.AddConsumer<SampleKafkaConsumer>();

            // TODO: มีการใช้ Request Client
            // การสร้าง Request Client ใน RabbitMQ ดูได้ที่
            // configure.AddRequestClient<DebtCancel>();

            return configure;
        }

        public static IEnumerable<KafkaConsumerSetting> ConfigKafkaConsumer(string projectName)
        {
            // TODO: ใส่ Consumer ของ Kafka ที่นี่
            // การสร้าง Consumer ใน Kafka ดูได้ที่
            // yield return new KafkaConsumerSetting<SampleKafkaConsumer,Ignore,Null>("SampleTopic", projectName, AutoOffsetReset.Earliest);

            // TODO: กรณีที่ใช้ Kafka Consumer เอาบรรทัดด้านล่างออก
            return null;
        }

        public static IEnumerable<KafkaProducerSetting> ConfigKafkaProducer()
        {
            // TODO: ใส่ Producer ของ Kafka ที่นี่
            // การสร้าง Producer ใน Kafka ดูได้ที่
            // yield return new KafkaProducerSetting<SampleMessage>("SampleTopic");

            // TODO: กรณีที่ใช้ Kafka Producer เอาบรรทัดด้านล่างออก
            return null;
        }
    }
}