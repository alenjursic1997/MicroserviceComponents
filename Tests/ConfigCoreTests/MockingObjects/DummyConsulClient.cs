using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ConfigCoreTests.MockingObjects
{
	public class DummyConsulClient : IConsulClient
	{
		public IACLEndpoint ACL => throw new NotImplementedException();

		public IAgentEndpoint Agent => throw new NotImplementedException();

		public ICatalogEndpoint Catalog => throw new NotImplementedException();

		public IEventEndpoint Event => throw new NotImplementedException();

		public IHealthEndpoint Health => throw new NotImplementedException();

		public IKVEndpoint KV => new DummyKV();

		public IRawEndpoint Raw => throw new NotImplementedException();

		public ISessionEndpoint Session => throw new NotImplementedException();

		public IStatusEndpoint Status => throw new NotImplementedException();

		public IOperatorEndpoint Operator => throw new NotImplementedException();

		public IPreparedQueryEndpoint PreparedQuery => throw new NotImplementedException();

		public ICoordinateEndpoint Coordinate => throw new NotImplementedException();

		public ISnapshotEndpoint Snapshot => throw new NotImplementedException();

		public Task<IDistributedLock> AcquireLock(LockOptions opts, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDistributedLock> AcquireLock(string key, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDistributedSemaphore> AcquireSemaphore(SemaphoreOptions opts, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<IDistributedSemaphore> AcquireSemaphore(string prefix, int limit, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public IDistributedLock CreateLock(LockOptions opts)
		{
			throw new NotImplementedException();
		}

		public IDistributedLock CreateLock(string key)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public Task ExecuteInSemaphore(SemaphoreOptions opts, Action a, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteInSemaphore(string prefix, int limit, Action a, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteLocked(LockOptions opts, Action action, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteLocked(LockOptions opts, CancellationToken ct, Action action)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteLocked(string key, Action action, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task ExecuteLocked(string key, CancellationToken ct, Action action)
		{
			throw new NotImplementedException();
		}

		public IDistributedSemaphore Semaphore(SemaphoreOptions opts)
		{
			throw new NotImplementedException();
		}

		public IDistributedSemaphore Semaphore(string prefix, int limit)
		{
			throw new NotImplementedException();
		}
	}
}
