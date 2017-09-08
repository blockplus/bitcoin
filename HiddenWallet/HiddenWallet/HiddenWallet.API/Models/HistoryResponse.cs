﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenWallet.API.Models
{
    public class HistoryResponse : BaseResponse
    {
		public HistoryResponse() => Success = true;
		public HistoryRecordModel[] History { get; set; }
	}
}
