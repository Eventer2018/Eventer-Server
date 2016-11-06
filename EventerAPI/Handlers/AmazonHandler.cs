using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;

namespace EventerAPI.Handlers
{
    public class AmazonHandler : ApiController
    {
        private static string aws_access_key = "AKIAJGX5RHJOVZIUCB6A";
        private static string aws_secret_key = "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";
        private static string bucket_name = "eventer-videos";

        public string PutFile(string file_path, string file_name) {

            string fname = string.IsNullOrWhiteSpace(file_name) ? Guid.NewGuid().ToString().Split('-')[0] + DateTime.Now.ToString("_yyyyMMddHHmm") + Path.GetExtension(file_path) : file_name;
            AmazonS3Client as3client = new AmazonS3Client(aws_access_key, aws_secret_key, RegionEndpoint.USEast1);

            PutObjectRequest pureq = new PutObjectRequest();
            pureq.FilePath = file_path;
            pureq.BucketName = bucket_name;
            //pureq.InputStream = f.InputStream;
            pureq.Key = fname;
            pureq.CannedACL = S3CannedACL.PublicRead;

            PutObjectResponse pures = new PutObjectResponse();
            pures = as3client.PutObject(pureq);

            return fname;
        
        }
    }
}
