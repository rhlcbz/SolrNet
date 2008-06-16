using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.DSL.Tests {
	/// <summary>
	/// These tests are more to define DSL syntax than anything else.
	/// </summary>
	[TestFixture]
	public class DSLTests {
		public class TestDocument : ISolrDocument {}

		public class TestDocumentWithId : ISolrDocument {
			[SolrField]
			public int Id { get; set; }
		}

		private const string response =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

		public delegate string Writer(string s, IDictionary<string, string> q);

		[Test]
		public void Add() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", "<add><doc /></add>")).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Add(new TestDocument());
			});
		}

		[Test]
		public void Commit() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Commit();
		}

		[Test]
		public void CommitWithParams() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Commit(true, true);
		}

		[Test]
		public void DeleteById() {
			const string id = "123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id></delete>", id))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ById(id);
			});
		}

		[Test]
		public void DeleteByQuery() {
			const string q = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ByQuery(new SolrQuery(q));
			});
		}

		[Test]
		public void DeleteByQueryString() {
			const string q = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ByQuery(q);
			});
		}

		[Test]
		public void Optimize() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Optimize();
		}

		[Test]
		public void OptimizeWithParams() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Optimize(true, true);
		}

		[Test]
		public void OrderBy() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "Id:123456"}, {"sort", "id asc"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var doc = new TestDocumentWithId {Id = 123456};
				Solr.Query<TestDocumentWithId>()
					.ByExample(doc)
					.OrderBy("id")
					.Run();
			});
		}

		[Test]
		public void OrderBy2() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			const string queryString = "id:123";
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", queryString}, {"sort", "id asc"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>(new SolrQuery(queryString), new SortOrder("id", Order.ASC));
			});
		}

		[Test]
		public void OrderBy2Multiple() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			const string queryString = "id:123";
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", queryString}, {"sort", "id asc,name desc"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>(new SolrQuery(queryString), new[] {new SortOrder("id", Order.ASC), new SortOrder("name", Order.DESC)});
			});
		}

		[Test]
		public void OrderByAscDesc() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "Id:123456"}, {"sort", "id asc"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var doc = new TestDocumentWithId {Id = 123456};
				Solr.Query<TestDocumentWithId>()
					.ByExample(doc)
					.OrderBy("id", Order.ASC)
					.Run();
			});
		}

		[Test]
		public void OrderByAscDescMultiple() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string>
				            	{
				            		{"q", "Id:123456"},
				            		{"sort", "id asc,name desc"}
				            	};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var doc = new TestDocumentWithId {Id = 123456};
				Solr.Query<TestDocumentWithId>()
					.ByExample(doc)
					.OrderBy("id", Order.ASC)
					.OrderBy("name", Order.DESC)
					.Run();
			});
		}

		[Test]
		public void Query() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var r = Solr.Query<TestDocument>("");
				Assert.AreEqual(1, r.NumFound);
			});
		}

		[Test]
		public void Query_InvalidField_ShouldNOTThrow() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(
					@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>("");
			});
		}

		[Test]
		public void QueryByAnyField() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:123456"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().By("id").Is("123456").Run();
			});
		}

		[Test]
		public void QueryByExample() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "Id:123456"}};
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var doc = new TestDocumentWithId {Id = 123456};
				Solr.Query<TestDocumentWithId>().ByExample(doc).Run();
			});
		}

		[Test]
		public void QueryByRange() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:[123 TO 456]"}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().ByRange("id", 123, 456).Run();
			});
		}

		[Test]
		public void QueryByRange_AnotherSyntax() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:[123 TO 456]"}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().By("id").Between(123).And(456).Run();
			});
		}

		[Test]
		public void QueryByRangeConcatenable() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:[123 TO 456] p:[a TO z]"}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().ByRange("id", 123, 456).ByRange("p", "a", "z").Run();
			});
		}

		[Test]
		public void QueryByRangeExclusive() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:{123 TO 456}"}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().ByRange("id", 123, 456).Exclusive().Run();
			});
		}

		[Test]
		public void QueryByRangeInclusive() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", "id:[123 TO 456]"}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>().ByRange("id", 123, 456).Inclusive().Run();
			});
		}

		[Test]
		public void QueryISolrQuery() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			const string queryString = "id:123";
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string> {{"q", queryString}};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>(new SolrQuery(queryString));
			});
		}

		[Test]
		public void QueryISolrQueryWithPagination() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			const string queryString = "id:123";
			With.Mocks(mocks).Expecting(delegate {
				var query = new Dictionary<string, string>
				            	{
				            		{"q", queryString},
				            		{"start", 10.ToString()},
				            		{"rows", 20.ToString()}
				            	};
				Expect.Call(conn.Get("/select", query))
					.Repeat.Once()
					.Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Query<TestDocument>(new SolrQuery(queryString), 10, 20);
			});
		}

		[Test]
		public void QueryWithPagination() {
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				var r = Solr.Query<TestDocument>("", 10, 20);
				Assert.AreEqual(1, r.NumFound);
			});
		}
	}
}