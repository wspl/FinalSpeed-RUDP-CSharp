using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class SendRecord
    {
        public int SendSize { get; set; }
        public int SendSizeFirst { get; set; }
        public int SendCount { get; set; }

        private int _ackedSize;
        public int AckedSize
        {
            get
            {
                return _ackedSize;
            }
            set
            {
                if (value > _ackedSize)
                {
                    _ackedSize = value;
                }
            }
        }

        public int TimeId { get; set; }

        private int _speed;
        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
                SpeedRecord = true;
            }
        }

        public bool SpeedRecord = false;

        public int Resent { get; set; }

        public float DropRate
        {
            get
            {
                int dropped = SendSize - AckedSize;
                if (dropped < 0)
                {
                    dropped = 0;
                }
                float dropRate = 0;
                if (SendSize > 0)
                {
                    dropRate = (float)dropped / SendSize;
                }
                return dropRate;
            }
        }

        public float ResendRate
        {
            get
            {
                float resendRate = 0;
                if (SendSizeFirst > 0)
                {
                    resendRate = (float)Resent / SendSizeFirst;
                }
                return resendRate;
            }
        }

        public void AddResent(int size) => Resent += size;

        public void AddSent(int size)
        {
            SendCount += 1;
            SendSize += size;
        }

        public void AddSentFirst(int size) => SendSizeFirst += size;
    }
}
