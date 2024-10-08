﻿using TaskManager.Enum;

namespace TaskManager.Response;

public class BaseResponse<T> : IBaseResponse<T>
{
    public string Description { get; set; }
    public T Data { get; set; }
    public StatusCode StatusCode { get; set; }
};
