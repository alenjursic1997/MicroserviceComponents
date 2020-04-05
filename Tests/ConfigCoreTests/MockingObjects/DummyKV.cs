using Consul;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ConfigCoreTests.MockingObjects
{
	public class DummyKV : IKVEndpoint
	{
		public Task<WriteResult<bool>> Acquire(KVPair p, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Acquire(KVPair p, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> CAS(KVPair p, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> CAS(KVPair p, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Delete(string key, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Delete(string key, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> DeleteCAS(KVPair p, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> DeleteCAS(KVPair p, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> DeleteTree(string prefix, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> DeleteTree(string prefix, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<KVPair>> Get(string key, CancellationToken ct = default)
		{
			var queryRes = new QueryResult<KVPair>();
			var pair = new KVPair(key);
			queryRes.Response = pair;

			if (key == "key/test1")
			{
				queryRes.Response.Value = new UTF8Encoding().GetBytes("value1");
			}
			else
			{
				queryRes.Response.Value = null;
			}

			return Task.FromResult(queryRes);
		}

		public Task<QueryResult<KVPair>> Get(string key, QueryOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<string[]>> Keys(string prefix, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<string[]>> Keys(string prefix, string separator, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<string[]>> Keys(string prefix, string separator, QueryOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<KVPair[]>> List(string prefix, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<KVPair[]>> List(string prefix, QueryOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Put(KVPair p, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Put(KVPair p, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Release(KVPair p, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<bool>> Release(KVPair p, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<KVTxnResponse>> Txn(List<KVTxnOp> txn, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<WriteResult<KVTxnResponse>> Txn(List<KVTxnOp> txn, WriteOptions q, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}
	}
}
