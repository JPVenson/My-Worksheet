//using System;
//using System.Collections.Generic;
//using System.Web.Http;
//using Katana.CommonTasks.Services.Logging.Contracts;
//using MyWorksheet.Webpage.Entities.Manager;
//using MyWorksheet.Webpage.Entities.Poco;
//using MyWorksheet.Webpage.PaymentProviderHelper;
//using MyWorksheet.Webpage.PaymentProviderHelper.PaymentHandlers;

//namespace MyWorksheet.Webpage.Controller.Api.Payment
//{
//	[Route(PayPalHandler.PayPalController)]
//	public class PayPalPaymentApiControllerBase : ApiControllerBase
//	{
//		private readonly DbEntities _db;
//		private readonly FeatureOrderHandler _featureHandler;
//		private readonly IAppLogger _logger;

//		public PayPalPaymentApiControllerBase(DbEntities db, FeatureOrderHandler featureHandler, IAppLogger logger)
//		{
//			_db = db;
//			_featureHandler = featureHandler;
//			_logger = logger;
//		}

//		[HttpGet]
//		[Route("CancelPayment")]
//		public IActionResult CancelPayment(Guid orderId)
//		{
//			_logger.LogInformation("Cancled Feature", LoggerCategorys.Feature.ToString(), new Dictionary<string, string>() { { "OrderId", orderId.ToString() } });
//			var resultRedict = Redirect(new Uri("/#!/RestrictedArea/UserSettings/false//" + orderId, UriKind.Relative));
//			var order = _db.Select<PaymentOrder>(orderId);
//			if (order == null || order.IsOrderDone)
//			{
//				return resultRedict;
//			}
//			try
//			{
//				order.IsOrderSuccess = false;
//				order.IsOrderDone = true;
//				order.OrderResolveDate = DateTime.UtcNow;
//				order.OrderError = "Canceled by User";
//				_db.Update(order);
//			}
//			catch (Exception e)
//			{
//				return resultRedict;
//			}
//			return resultRedict;
//		}

//		[HttpGet]
//		[Route("Check")]
//		public IActionResult ConfirmPayment(string paymentId, string PayerID, Guid orderId)
//		{
//			_logger.LogInformation("Payed Feature", LoggerCategorys.Feature.ToString(), new Dictionary<string, string>()
//			{
//				{ "OrderId", orderId.ToString() },
//				{ "paymentId", paymentId.ToString() }
//			});

//			var resultRedict = Redirect(new Uri("/#!/RestrictedArea/UserSettings/false//" + orderId, UriKind.Relative));

//			var order = _db.Select<PaymentOrder>(orderId);
//			if (order == null || order.IsOrderDone)
//			{
//				return resultRedict;
//			}
//			var context = PayPalHandler.GetContext();

//			var paymentExecution = new PayPal.Api.PaymentExecution() { payer_id = PayerID };
//			var payment = new PayPal.Api.Payment() { id = paymentId };
//			try
//			{
//				var execute = payment.Execute(context, paymentExecution);
//				if (execute.state == "approved")
//				{
//					var isSuccess = _featureHandler.ActiveOrder(orderId);
//				}
//				else
//				{
//					order.IsOrderSuccess = false;
//					order.IsOrderDone = true;
//					order.OrderResolveDate = DateTime.UtcNow;
//					order.OrderError = execute.failure_reason;
//					_db.Update(order);
//				}
//			}
//			catch (Exception e)
//			{
//				return resultRedict;
//			}
//			return resultRedict;
//		}
//	}
//}