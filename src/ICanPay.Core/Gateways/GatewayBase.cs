using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace ICanPay.Core
{
    /// <summary>
    /// ֧�����صĳ������
    /// </summary>
    public abstract class GatewayBase
    {
        #region �����ֶ�

        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string SUCCESS = "success";
        public const string FAILURE = "failure";
        public const string FAIL = "FAIL";
        public const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        #endregion

        #region ˽���ֶ�

        private GatewayData gatewayData;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯��
        /// </summary>
        protected GatewayBase()
            : this(new GatewayData())
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="merchant">�̻�����</param>
        protected GatewayBase(IMerchant merchant)
            : this(new GatewayData())
        {
            Merchant = merchant;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="gatewayData">��������</param>
        protected GatewayBase(GatewayData gatewayData)
        {
            this.gatewayData = gatewayData;
        }

        #endregion

        #region ����

        /// <summary>
        /// ��������
        /// </summary>
        public IOrder Order { get; set; }

        /// <summary>
        /// �̻�����
        /// </summary>
        public IMerchant Merchant { get; set; }

        /// <summary>
        /// ֪ͨ����
        /// </summary>
        public INotify Notify { get; set; }

        /// <summary>
        /// ֧�����ص�����
        /// </summary>
        public abstract GatewayType GatewayType { get; }

        /// <summary>
        /// ֧�����صĵ�ַ
        /// </summary>
        public abstract string GatewayUrl { get; }

        /// <summary>
        /// ֧�����صĽ�������
        /// </summary>
        public GatewayTradeType GatewayTradeType { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public GatewayData GatewayData
        {
            get
            {
                return gatewayData;
            }
            set
            {
                gatewayData = value;
            }
        }

        #endregion

        #region ����

        /// <summary>
        /// ��֤�����Ƿ�֧���ɹ�
        /// </summary>
        internal async Task<bool> ValidateNotifyAsync()
        {
            if (await CheckNotifyDataAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ��֤����
        /// </summary>
        /// <param name="instance">��֤����</param>
        private void ValidateParameter(object instance)
        {
            var validationContext = new ValidationContext(instance);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instance, validationContext, results, true);

            if (!isValid)
            {
                throw new Exception(results[0].ErrorMessage);
            }
        }

        /// <summary>
        /// �������ط��ص�֪ͨ��ȷ�϶����Ƿ�֧���ɹ�
        /// </summary>
        protected abstract Task<bool> CheckNotifyDataAsync();

        /// <summary>
        /// �����յ�֧������֪ͨ����֤����ʱ����֧������Ҫ���ʽ�����ʾ�ɹ����յ�����֪ͨ���ַ���
        /// </summary>
        public virtual void WriteSuccessFlag()
        {
            HttpUtil.Write(SUCCESS);
        }

        /// <summary>
        /// �����յ�֧������֪ͨ����֤����ʱ����֧������Ҫ���ʽ�����ʾʧ�ܽ��յ�����֪ͨ���ַ���
        /// </summary>
        public virtual void WriteFailureFlag()
        {
            HttpUtil.Write(FAILURE);
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        protected virtual void InitOrderParameter()
        {
            SupplementaryParameter();
        }

        /// <summary>
        /// ���䲻֧ͬ�����͵�ȱ�ٲ���
        /// </summary>
        private void SupplementaryParameter()
        {
            if (GatewayTradeType == GatewayTradeType.App)
            {
                SupplementaryAppParameter();
            }

            if (GatewayTradeType == GatewayTradeType.Scan)
            {
                SupplementaryScanParameter();
            }

            if (GatewayTradeType == GatewayTradeType.Wap)
            {
                SupplementaryWapParameter();
            }

            if (GatewayTradeType == GatewayTradeType.Web)
            {
                SupplementaryWebParameter();
            }

            ValidateParameter(Merchant);
            ValidateParameter(Order);
        }

        /// <summary>
        /// �����ƶ�֧����ȱ�ٲ���
        /// </summary>
        protected abstract void SupplementaryAppParameter();

        /// <summary>
        /// ���������վ֧����ȱ�ٲ���
        /// </summary>
        protected abstract void SupplementaryWebParameter();

        /// <summary>
        /// �����ֻ���վ֧����ȱ�ٲ���
        /// </summary>
        protected abstract void SupplementaryWapParameter();

        /// <summary>
        /// ����ɨ��֧����ȱ�ٲ���
        /// </summary>
        protected abstract void SupplementaryScanParameter();

        #endregion

    }
}