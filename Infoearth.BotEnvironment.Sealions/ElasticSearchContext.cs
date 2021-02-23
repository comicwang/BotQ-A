using Newtonsoft.Json;
using PlainElastic.Net;
using PlainElastic.Net.Mappings;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.BotEnvironment.Sealions
{
    public class ElasticSearchContext
    {
        public static readonly ElasticSearchContext Intance = new ElasticSearchContext();
        private ElasticConnection Client;
        private ElasticSearchContext()
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["ElasticSearchConnection"];
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("ES连接配置错误");
            string[] urls = url.Split(' ');
            Client = new ElasticConnection(urls[0], urls.Length > 1 ? int.Parse(urls[1]) : 9200);
        }
        /// <summary>
        /// 数据索引
        /// </summary>       
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="id">索引文档id，不能重复,如果重复则覆盖原先的</param>
        /// <param name="jsonDocument">要索引的文档,json格式</param>
        /// <returns></returns>
        public IndexResult Index<T>(string indexName, string indexType, string id, T jsonDocument) where T : BaseHighlight
        {

            var serializer = new JsonNetSerializer();
            string cmd = new IndexCommand(indexName, indexType, id);

            List<string> lstIgnoreColumns = new List<string>();
            PropertyInfo[] propertys = typeof(T).GetProperties();
            for (int j = 0; j < propertys.Length; j++)
            {
                var exportAttr = propertys[j].GetCustomAttributes<ColumnAttribute>();
                if (exportAttr.Any(t => t.Ignore))
                {
                    lstIgnoreColumns.Add(propertys[j].Name);
                }
            }
            string document = Newtonsoft.Json.JsonConvert.SerializeObject(jsonDocument, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new ExcludePropertiesContractResolver(lstIgnoreColumns)
            });

            OperationResult result = Client.Post(cmd, document);

            var indexResult = serializer.ToIndexResult(result.Result);
            return indexResult;
        }

        //public IndexResult Index(string indexName, string indexType, string id, object document)
        //{
        //    var serializer = new JsonNetSerializer();
        //    var jsonDocument = serializer.Serialize(document);
        //    return Index(indexName, indexType, id, jsonDocument);
        //}

        //全文检索，单个字段或者多字段 或关系
        //字段intro 包含词组key中的任意一个单词
        /// <summary>
        /// 搜索索引信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="intro">搜索属性名称（默认不填为所有）</param>
        /// <param name="opera">分词匹配模式（默认不填为所有词匹配）</param>
        /// <returns></returns>
        public ElasticModel<T> Search<T>(string indexName, string indexType, string key, string intro = null, Operator opera = Operator.AND) where T : BaseHighlight
        {
            string cmd = new SearchCommand(indexName, indexType);
            QueryString<T> queryString = new QueryString<T>();

            QueryBuilder<T> querybuild = new QueryBuilder<T>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                //并且关系
                                m.Must(t =>

                                   //分词的最小单位或关系查询
                                   t.QueryString(t1 => t1.Query(key).DefaultOperator(opera))
                                     //.QueryString(t1 => t1.DefaultField("name").Query(key))
                                     // t .Terms(t2=>t2.Field("intro").Values("研究","方鸿渐"))
                                     //范围查询
                                     // .Range(r =>  r.Field("age").From("100").To("200") )  
                                     )
                                  )
                                );
            if (!string.IsNullOrEmpty(intro))
            {
                querybuild = new QueryBuilder<T>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                //并且关系
                                m.Must(t =>

                                   //分词的最小单位或关系查询
                                   t.QueryString(t1 => t1.DefaultField(intro).Query(key).DefaultOperator(opera))
                                     //.QueryString(t1 => t1.DefaultField("name").Query(key))
                                     // t .Terms(t2=>t2.Field("intro").Values("研究","方鸿渐"))
                                     //范围查询
                                     // .Range(r =>  r.Field("age").From("100").To("200") )  
                                     )
                                  )
                                );
            }
            //排序
            // .Sort(c => c.Field("age", SortDirection.desc))
            //添加高亮
            string query = querybuild.Highlight(h => h
                    .PreTags("<b>")
                    .PostTags("</b>")
                    .Fields(
                           f => f.FieldName(intro).Order(HighlightOrder.score),
                           f => f.FieldName("_all")
                           )
                 )
                .Build();

            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<T>(result);
            ElasticModel<T> datalist = new ElasticModel<T>();
            datalist.hits = list.hits.total.value;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c => c._source).ToArray();
            //获取高亮值
            for (int i = 0; i < personList.Length; i++)
            {
                if (list.hits.hits[i].highlight != null)
                    personList[i].highlight = string.Join("", list.hits.hits[i].highlight.Select(t => t.Value.FirstOrDefault()));
                personList[i]._id = list.hits.hits[i]._id;
                personList[i]._score = list.hits.hits[i]._score;
            }
            datalist.list.AddRange(personList);
            return datalist;
        }

        /// <summary>
        /// 删除某条数据
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DeleteResult Delete(string indexName,string indexType,string id)
        {
            var serializer = new JsonNetSerializer();
            string cmd = new DeleteCommand(indexName, indexType, id);
            OperationResult operationResult = Client.Delete(cmd);
            var indexResult = serializer.ToDeleteResult(operationResult.Result);
            return indexResult;
        }

        //全文检索，多字段 并关系
        //字段intro 或者name 包含词组key
        public ElasticModel<T> SearchFullFileds<T>(string indexName, string indexType, string key, int from, int size) where T: BaseHighlight
        {
            MustQuery<T> mustNameQueryKeys = new MustQuery<T>();
            MustQuery<T> mustIntroQueryKeys = new MustQuery<T>();
            var arrKeys = GetIKTokenFromStr(key);
            foreach (var item in arrKeys)
            {
                mustNameQueryKeys = mustNameQueryKeys.Term(t3 => t3.Field("name").Value(item)) as MustQuery<T>;
                mustIntroQueryKeys = mustIntroQueryKeys.Term(t3 => t3.Field("intro").Value(item)) as MustQuery<T>;
            }

            string cmd = new SearchCommand(indexName, indexType);
            string query = new QueryBuilder<T>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                m.Should(t =>
                                         t.Bool(m1 =>
                                                     m1.Must(
                                                             t2 =>
                                                                 //t2.Term(t3=>t3.Field("name").Value("研究"))
                                                                 //   .Term(t3=>t3.Field("name").Value("方鸿渐"))  
                                                                 mustNameQueryKeys
                                                             )
                                                )
                                       )
                               .Should(t =>
                                         t.Bool(m1 =>
                                                     m1.Must(t2 =>
                                                                     //t2.Term(t3 => t3.Field("intro").Value("研究"))
                                                                     //.Term(t3 => t3.Field("intro").Value("方鸿渐"))  
                                                                     mustIntroQueryKeys
                                                            )
                                                )
                                      )
                                  )
                        )
                 //分页
                 .From(from)
                 .Size(size)
                  //排序
                  // .Sort(c => c.Field("age", SortDirection.desc))
                  //添加高亮
                  .Highlight(h => h
                      .PreTags("<b>")
                      .PostTags("</b>")
                      .Fields(
                             f => f.FieldName("intro").Order(HighlightOrder.score),
                              f => f.FieldName("name").Order(HighlightOrder.score)
                             )
                     )
                    .Build();


            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<T>(result);
            ElasticModel<T> datalist = new ElasticModel<T>();
            datalist.hits = list.hits.total.value;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c=>c._source).ToList();
            //new T()
            //{
            //    id = c._source.id,
            //    age = c._source.age,
            //    birthday = c._source.birthday,
            //    intro = c.highlight == null || !c.highlight.Keys.Contains("intro") ? c._source.intro : string.Join("", c.highlight["intro"]), //高亮显示的内容，一条记录中出现了几次
            //    name = c.highlight == null || !c.highlight.Keys.Contains("name") ? c._source.name : string.Join("", c.highlight["name"]),
            //    sex = c._source.sex

            //});
            datalist.list.AddRange(personList);
            return datalist;
        }

        //全文检索，多字段 并关系
        //搜索age在100到200之间，并且字段intro 或者name 包含词组key
        public ElasticModel<T> SearchFullFiledss<T>(string indexName, string indexType, string key, int from, int size) where T: BaseHighlight
        {
            MustQuery<T> mustNameQueryKeys = new MustQuery<T>();
            MustQuery<T> mustIntroQueryKeys = new MustQuery<T>();
            var arrKeys = GetIKTokenFromStr(key);
            foreach (var item in arrKeys)
            {
                mustNameQueryKeys = mustNameQueryKeys.Term(t3 => t3.Field("name").Value(item)) as MustQuery<T>;
                mustIntroQueryKeys = mustIntroQueryKeys.Term(t3 => t3.Field("intro").Value(item)) as MustQuery<T>;
            }

            string cmd = new SearchCommand(indexName, indexType);
            string query = new QueryBuilder<T>()
                //1 查询
                .Query(b =>
                            b.Bool(m =>
                                m.Must(t =>
                                          t.Range(r => r.Field("age").From("1").To("500"))
                                          .Bool(ms =>
                                                    ms.Should(ts =>
                                                             ts.Bool(m1 =>
                                                                         m1.Must(
                                                                                 t2 =>
                                                                                      //t2.Term(t3=>t3.Field("name").Value("研究"))
                                                                                      //   .Term(t3=>t3.Field("name").Value("方鸿渐"))  
                                                                                      //
                                                                                      mustNameQueryKeys
                                                                                 )
                                                                    )
                                                           )
                                                   .Should(ts =>
                                                             ts.Bool(m1 =>
                                                                         m1.Must(t2 =>
                                                                                        //t2.Term(t3 => t3.Field("intro").Value("研究"))
                                                                                        //.Term(t3 => t3.Field("intro").Value("方鸿渐"))  

                                                                                        //
                                                                                        mustIntroQueryKeys
                                                                                )
                                                                    )
                                                          )
                                                      )
                                                        )
                                                      )
                       )
                 //分页
                 .From(from)
                 .Size(size)
                  //排序
                  // .Sort(c => c.Field("age", SortDirection.desc))
                  //添加高亮
                  .Highlight(h => h
                      .PreTags("<b>")
                      .PostTags("</b>")
                      .Fields(
                             f => f.FieldName("intro").Order(HighlightOrder.score),
                              f => f.FieldName("name").Order(HighlightOrder.score)
                             )
                     )
                    .Build();


            string result = Client.Post(cmd, query);
            var serializer = new JsonNetSerializer();
            var list = serializer.ToSearchResult<T>(result);
            ElasticModel<T> datalist = new ElasticModel<T>();
            datalist.hits = list.hits.total.value;
            datalist.took = list.took;
            var personList = list.hits.hits.Select(c => c._source).ToList();
            //Select(c => new Supernova.Webapi.DbHelper.person()
            //{
            //    id = c._source.id,
            //    age = c._source.age,
            //    birthday = c._source.birthday,
            //    intro = c.highlight == null || !c.highlight.Keys.Contains("intro") ? c._source.intro : string.Join("", c.highlight["intro"]), //高亮显示的内容，一条记录中出现了几次
            //    name = c.highlight == null || !c.highlight.Keys.Contains("name") ? c._source.name : string.Join("", c.highlight["name"]),
            //    sex = c._source.sex

            //});
            datalist.list.AddRange(personList);
            return datalist;


        }
        //分词映射
        private static string BuildCompanyMapping()
        {
            return new MapBuilder<Model>()
                .RootObject(typeName: "person",
                            map: r => r
                    .All(a => a.Enabled(false))
                    .Dynamic(false)
                    .Properties(pr => pr
                        .String(person => person.name, f => f.Analyzer(DefaultAnalyzers.standard).Boost(2))
                        .String(person => person.intro, f => f.Analyzer("ik"))
                        )
              )
              .BuildBeautified();
        }

        //将语句用ik分词，返回分词结果的集合
        private List<string> GetIKTokenFromStr(string key)
        {
            string s = "/db_test/_analyze?analyzer=ik";
            var result = Client.Post(s, "{" + key + "}");
            var serializer = new JsonNetSerializer();
            var list = serializer.Deserialize(result, typeof(ik)) as ik;
            return list.tokens.Select(c => c.token).ToList();
        }


    }
}
