using System;

namespace RoboBank.BankingPortal.Application
{
    public class NotFoundException : Exception
    {
        public NotFoundException (string message): base (message)
        {
        }
    }
}