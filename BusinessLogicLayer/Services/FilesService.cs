using BusinessLogicLayer.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class FilesService : IFilesService
    {
        public void Upload(string Destination, IFormFile file)
        {
            if (file != null && file.Length > 0) 
            {
                using(FileStream fs = new FileStream(Destination, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
            }
        }
    }
}
