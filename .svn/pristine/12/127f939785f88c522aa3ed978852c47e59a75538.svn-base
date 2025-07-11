using System;
using System.Linq;
using Core.Database;
using Core.Entity.MyCore;

namespace Core.UserDataProviders.Session
{
    public class SessionDataProvider : ISessionDataProvider
    {
        private readonly MyCoreContext _context;

        public SessionDataProvider(MyCoreContext context)
        {
            _context = context;
        }

        public bool SetSession(string userId, bool isClosing)
        {
            try
            {
                Core.Entity.MyCore.Session? session = null;
                if (isClosing)
                {
                    session = _context.Session.FirstOrDefault(x => x.UserId.Equals(userId) && x.ClosedOn == null);
                    if (session == null)
                    {
                        session = new Core.Entity.MyCore.Session
                        {
                            Id = Guid.NewGuid().ToString().ToUpper(),
                            UserId = userId,
                            ClosedOn = DateTime.Now
                        };
                        _context.Session.Add(session);
                        _context.SaveChanges();
                    }
                    else
                    {
                        session.ClosedOn = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    session = new Core.Entity.MyCore.Session
                    {
                        Id = Guid.NewGuid().ToString().ToUpper(),
                        UserId = userId,
                        OpenedOn = DateTime.Now
                    };
                    _context.Session.Add(session);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}