using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using ApiKarbord.Controllers.Unit;
using ApiKarbord.Models;
using Newtonsoft.Json;

namespace ApiKarbord.Controllers.AFI.data
{
    public class AFI_FDocDataController : ApiController
    {

        // GET: api/FDocData/FMode اطلاعات نوع سند حسابداری   
        [Route("api/FDocData/FMode/{userName}/{password}/{ace}/{sal}/{group}")]
        public async Task<IHttpActionResult> GetWeb_FMode(string userName, string password, string ace, string sal, string group)
        {
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                string sql = "SELECT * FROM Web_FMode";
                var listFMode = UnitDatabase.db.Database.SqlQuery<Web_FMode>(sql);
                return Ok(listFMode);
            }
            return null;
        }


        // GET: api/FDocData/FDocH اطلاعات تکمیلی فاکتور    
        [Route("api/FDocData/FDocH/{userName}/{password}/{ace}/{sal}/{group}/{serialNumber}/{ModeCode}")]
        public async Task<IQueryable<Web_FDocH>> GetWeb_FDocH(string userName, string password, string ace, string sal, string group, long serialNumber, string ModeCode)
        {
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                var a = UnitDatabase.db.Web_FDocH.Where(c => c.SerialNumber == serialNumber && c.ModeCode == ModeCode);
                return a;
            }
            return null;
        }


        // GET: api/FDocData/FDocH لیست فاکتور    


        [Route("api/FDocData/FDocH/{userName}/{password}/{ace}/{sal}/{group}/{ModeCode}/top{select}/{user}/{AccessSanad}")]
        public async Task<IHttpActionResult> GetAllWeb_FDocHMin(string userName, string password, string ace, string sal, string group, string ModeCode, int select, string user, bool accessSanad)
        {

            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {

                string sql = "select ";
                if (select == 0)
                    sql += " top(100) ";
                sql += string.Format(@"SerialNumber,                                   
                                       DocNo,
                                       SortDocNo,
                                       DocDate,
                                       CustCode,
                                       CustName,
                                       Spec,
                                       KalaPriceCode,
                                       InvCode,
                                       AddMinSpec1,
                                       AddMinSpec2,
                                       AddMinSpec3,
                                       AddMinSpec4,
                                       AddMinSpec5,
                                       AddMinSpec6,
                                       AddMinSpec7,
                                       AddMinSpec8,
                                       AddMinSpec9,
                                       AddMinSpec10,
                                       AddMinPrice1,
                                       AddMinPrice2,
                                       AddMinPrice3,
                                       AddMinPrice4,
                                       AddMinPrice5,
                                       AddMinPrice6,
                                       AddMinPrice7,
                                       AddMinPrice8,
                                       AddMinPrice9,
                                       AddMinPrice10,
                                       ModeCode,
                                       Status,
                                       PaymentType,
                                       Footer,
                                       Tanzim,
                                       Taeed,
                                       FinalPrice,
                                       Eghdam
                                       from Web_FDocH where ModeCode = '{0}' ",
                                       ModeCode.ToString());
                if (accessSanad == false)
                    sql += " and Eghdam = '" + user + "' ";
                sql += " order by SortDocNo desc ";
                var listFDocH = UnitDatabase.db.Database.SqlQuery<Web_FDocHMini>(sql);
                return Ok(listFDocH);
            }
            return null;
        }

        // GET: api/FDocData/FDocH تعداد رکورد ها    
        [Route("api/FDocData/FDocH/{userName}/{password}/{ace}/{sal}/{group}/{ModeCode}")]
        public async Task<IHttpActionResult> GetWeb_FDocHCount(string userName, string password, string ace, string sal, string group, string ModeCode)
        {
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                string sql = string.Format(@"SELECT count(SerialNumber) FROM Web_FDocH WHERE ModeCode = '{0}'", ModeCode);
                int count = UnitDatabase.db.Database.SqlQuery<int>(sql).Single();
                return Ok(count);
            }
            return null;
        }

        // GET: api/FDocData/FDocH آخرین تاریخ فاکتور    
        [Route("api/FDocData/FDocH/LastDate/{ace}/{sal}/{group}/{ModeCode}")]
        public async Task<IHttpActionResult> GetWeb_FDocHLastDate(string userName, string password, string ace, string sal, string group, string ModeCode)
        {
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                string lastdate;
                string sql = string.Format(@"SELECT count(DocDate) FROM Web_FDocH WHERE ModeCode = '{0}' ", ModeCode);
                int count = UnitDatabase.db.Database.SqlQuery<int>(sql).Single();
                if (count > 0)
                {
                    sql = string.Format(@"SELECT top(1) DocDate FROM Web_FDocH WHERE ModeCode = '{0}' order by DocDate desc", ModeCode);
                    lastdate = UnitDatabase.db.Database.SqlQuery<string>(sql).Single();
                }
                else
                    lastdate = "";
                return Ok(lastdate);
            }
            return null;
        }

        // GET: api/FDocData/FDocB اطلاعات تکمیلی فاکتور    
        [Route("api/FDocData/FDocB/{userName}/{password}/{ace}/{sal}/{group}/{serialNumber}")]
        public async Task<IHttpActionResult> GetWeb_FDocB(string userName, string password, string ace, string sal, string group, long serialNumber)
        {
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                string sql = string.Format(@"SELECT SerialNumber,BandNo,KalaCode,KalaName,MainUnit,MainUnitName,Amount1,Amount2,Amount3,UnitPrice,TotalPrice,Discount,Comm,Up_Flag,DeghatR1,DeghatR2,DeghatR3,DeghatM1,DeghatM2,DeghatM3,DeghatR
                                         FROM Web_FDocB WHERE SerialNumber = {0}", serialNumber);
                var listFactor = UnitDatabase.db.Database.SqlQuery<Web_FDocB>(sql);
                return Ok(listFactor);
            }
            return null;
        }

        [Route("api/FDocData/UpdatePrice/{userName}/{password}/{ace}/{sal}/{group}/{serialnumber}")]
        [ResponseType(typeof(AFI_FDocBi))]
        public async Task<IHttpActionResult> PostWeb_UpdatePrice(string userName, string password, string ace, string sal, string group, long serialnumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (UnitDatabase.CreateConection(userName, password, ace, sal, group))
            {
                try
                {
                    string sql = string.Format(CultureInfo.InvariantCulture,
                          @"DECLARE	@return_value int
                            EXEC    @return_value = [dbo].[Web_FDocB_SetKalaPrice]
		                            @SerialNumber = {0}
                            SELECT  'Return Value' = @return_value",
                          serialnumber);
                    int value = UnitDatabase.db.Database.SqlQuery<int>(sql).Single();
                    if (value == 0)
                    {
                        await UnitDatabase.db.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            string sql1 = string.Format(@"SELECT SerialNumber,BandNo,KalaCode,KalaName,MainUnit,MainUnitName,Amount1,Amount2,Amount3,UnitPrice,TotalPrice,Discount,Comm,Up_Flag,DeghatR1,DeghatR2,DeghatR3,DeghatM1,DeghatM2,DeghatM3,DeghatR
                                          FROM Web_FDocB WHERE SerialNumber = {0}", serialnumber);
            var listFactor = UnitDatabase.db.Database.SqlQuery<Web_FDocB>(sql1);
            return Ok(listFactor);
        }

    }
}
