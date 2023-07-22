using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TableReservation;
using TableReservation.Mappers;
using TableReservation.Models;
using Twilio.Clients;
using Twilio.TwiML.Voice;
using Twilio.Types;

namespace Tests
{
    public class SetupTest : IDisposable
    {
        
        protected readonly IMapper _mapperConfig;
        protected readonly Fixture _fixture;
        protected readonly AppDbContext _dbContext;
        protected readonly IConfiguration _con;
        protected readonly Mock<ITwilioRestClient> _client;
        public static string connectionString = "workstation id=TableReservation.mssql.somee.com;packet size=4096;user id=Giacathuy0303_SQLLogin_1;pwd=l9lafk6mli;data source=TableReservation.mssql.somee.com;persist security info=False;initial catalog=TableReservation;TrustServerCertificate=True;;Trusted_Connection=False";
        public SetupTest()
        {


            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperCofig());
            });
            _mapperConfig = mappingConfig.CreateMapper();
            _client=new Mock<ITwilioRestClient>();
            _con  = new ConfigurationBuilder().Build();
            _fixture = new Fixture();
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlServer(connectionString)
                .Options;
            _dbContext = new AppDbContext(options);
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
