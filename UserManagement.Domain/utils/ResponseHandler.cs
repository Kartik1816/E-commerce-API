using Microsoft.AspNetCore.Http;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.utils;

public class ResponseHandler
{
    public ResponseModel Success(string Message, Object Data)
        {
            return new ResponseModel
            {
                IsSuccess = true,
                Message = Message,
                Data = Data,
                StatusCode = StatusCodes.Status200OK
            };
        }

        //Response #400
        public ResponseModel BadRequest(string ErrorCode,string Message,Object Data)
        {
            return new ResponseModel
            {
                IsSuccess = false,
                Message = Message,
                Data = Data,
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorCode = ErrorCode
            };
        }

        //Response #404
        public ResponseModel NotFoundRequest(string ErrorCode, string Message, Object Data)
        {
            return new ResponseModel
            {
                IsSuccess = false,
                Message = Message,
                Data = Data,
                StatusCode = StatusCodes.Status404NotFound,
                ErrorCode = ErrorCode
            };
        }
}
