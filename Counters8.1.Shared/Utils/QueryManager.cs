using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SQLite;
using Windows.Storage;

namespace Counters.Utils
{
    public class QueryManager
    {
        public const string dbVersion = "2.5";

        private AppSettings _settings = new AppSettings();
        private string _dbPath = System.IO.Path.Combine(System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "dbCounters.sqlite"));
        private SQLiteConnection _connection;
        private static readonly QueryManager _instance = new QueryManager();

        /// <summary>
        /// Синглетон
        /// </summary>
        public static QueryManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Соединение
        /// </summary>
        public SQLiteConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        protected QueryManager()
        {
            OpenConnection();
        }

        /// <summary>
        /// Открыть соединение
        /// </summary>
        public void OpenConnection()
        {
            _connection = new SQLiteConnection(_dbPath);
        }

        /// <summary>
        /// Обновить схему базы данных
        /// </summary>
        /// <returns></returns>
        public bool RefreshDbScheme()
        {
            try
            {
                while (_settings.dbVersionSetting != dbVersion)
                    switch (_settings.dbVersionSetting)
                    {
                        //8.07.2016
                        case "2.4":
                            var scores2 = _connection.Table<Score>().ToList();
                            _connection.DropTable<Score>();
                            _connection.CreateTable<Score>();
                            foreach (var s in scores2)
                            {
                                int scoreId = s.ScoreId;
                                _connection.Insert(s);
                                _connection.Execute($"Update Score Set ScoreId={scoreId} where ScoreId={s.ScoreId}");
                            }
                            _settings.dbVersionSetting = "2.5";
                            break;

                        //1.06.2016
                        case "2.3":
                            var datas = _connection.Query<CounterData>("Select * from CounterData");
                            _connection.DropTable<CounterData>();
                            _connection.CreateTable<CounterData>();

                            foreach (var d in datas)
                                if (d.Date.Date == new DateTime(2000, 1, 1))
                                    d.IsFirst = true;
                            _connection.InsertAll(datas);

                            _settings.dbVersionSetting = "2.4";
                            break;

                        //31.03.2016
                        case "2.2":
                            _connection.CreateTable<ServiceData>();

                            var scores = _connection.Table<Score>().ToList();
                            foreach (var s in scores)
                                if (!string.IsNullOrEmpty(s.lstServiceTarifs))
                                    foreach (var t in s.lstServiceTarifs.Split(','))
                                    {
                                        var service = _connection.Query<Service>(string.Format("select * from Service where ServiceId=(select ServiceId from ServiceTarif where TarifId={0})", t)).SingleOrDefault();
                                        if (service != null)
                                            _connection.Insert(new ServiceData
                                            {
                                                ScoreId = s.ScoreId,
                                                TarifId = int.Parse(t),
                                                ServiceId = service.ServiceId
                                            });
                                    }

                            var tarifs = _connection.Table<ServiceTarif>().ToList();
                            _connection.DropTable<ServiceTarif>();
                            _connection.CreateTable<ServiceTarif>();
                            _connection.InsertAll(tarifs);

                            _settings.dbVersionSetting = "2.3";
                            break;

                        default:
                            CreateDatabase();
                            return false;
                    }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Создать базу данных со всеми таблицами
        /// </summary>
        public void CreateDatabase()
        {
            // Удаляем таблицы, если уже есть
            _connection.DropTable<Counter>();
            _connection.DropTable<CounterData>();
            _connection.DropTable<Service>();
            _connection.DropTable<ServiceData>();
            _connection.DropTable<ServiceTarif>();
            _connection.DropTable<Score>();
            _connection.DropTable<Notification>();
            _connection.DropTable<Tarif>();
            _connection.DropTable<Property>();
            _connection.DropTable<Flat>();

            // Создаем заново
            _connection.CreateTable<Counter>();
            _connection.CreateTable<CounterData>();
            _connection.CreateTable<Service>();
            _connection.CreateTable<ServiceData>();
            _connection.CreateTable<ServiceTarif>();
            _connection.CreateTable<Score>();
            _connection.CreateTable<Notification>();
            _connection.CreateTable<Tarif>();
            _connection.CreateTable<Property>();
            _connection.CreateTable<Flat>();

            //Квартира по умолчанию
            var newFlat = new Flat() { Name = "Моя квартира" };
            _connection.Insert(newFlat);
            _settings.CurrentFlatId = newFlat.FlatId;
        }

        #region Counters

        /// <summary>
        /// Получить список счетчиков
        /// </summary>
        /// <param name="lstCounters">Список Id счетчиков</param>
        /// <returns></returns>
        public List<QueryResult> GetCountersByIds(string lstCounters)
        {
            return _connection.Query<QueryResult>(string.Format("select * from Counter where CounterId in ({0})", lstCounters));
        }

        /// <summary>
        /// Получить счетчик с тарифом
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="dataId">Id показаний, из которых берется тариф</param>
        /// <returns></returns>
        public QueryResult GetCounterWithTarif(int counterId, int dataId)
        {
            return _connection.Query<QueryResult>(string.Format("select * from Counter, Tarif where Counter.CounterId={0} and Tarif.TariffId={1}",
                counterId, dataId == 0 ? "Counter.TarifId" : $"(select TariffId from CounterData where DataId={dataId})")).Single();
        }

        /// <summary>
        /// Получить счетчик с показаниями
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="dataId">Id показаний</param>
        /// <returns></returns>
        public QueryResult GetCounterWithData(int counterId, int dataId)
        {
            if (dataId != 0)
                return _connection.Query<QueryResult>(string.Format("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                    + " join Tarif on CounterData.TariffId=Tarif.TariffId where CounterData.DataId={0}", dataId)).Single();
            else
                return _connection.Query<QueryResult>(string.Format("select * from Counter join Tarif on Counter.TarifId=Tarif.TariffId"
                    + " where Counter.CounterId={0}", counterId)).Single();
        }

        /// <summary>
        /// Установить новый тариф для показаний
        /// </summary>
        /// <param name="dataId">Id показаний</param>
        /// <param name="tarifId">Id тарифа</param>
        public void UpdateCounterDataTarif(int dataId, int tarifId)
        {
            _connection.Execute("Update CounterData set TariffId=? where DataId=?", tarifId, dataId);
        }

        /// <summary>
        /// Получить все показания счетчика
        /// </summary>
        /// <param name="id">Id счетчика</param>
        /// <returns></returns>
        public List<QueryResult> GetCounterDatas(int id)
        {
            return _connection.Query<QueryResult>("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                   + " join Tarif on CounterData.TariffId=Tarif.TariffId where Counter.CounterId=? order by Date desc", id);
        }

        /// <summary>
        /// Получить предыдущие показания
        /// </summary>
        /// <param name="currentData">Текущие показания</param>
        /// <returns></returns>
        public QueryResult GetPreviousData(QueryResult currentData)
        {
            return _connection.Query<QueryResult>("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                + " join Tarif on CounterData.TariffId=Tarif.TariffId where Counter.CounterId=? and DataId<>? and Date<? order by Date desc limit 1",
                currentData.CounterId, currentData.DataId, currentData.Date).FirstOrDefault();
        }

        /// <summary>
        /// Имеются ли показания счетчика за указанную дату
        /// </summary>
        /// <param name="data">Счетчик</param>
        /// <returns></returns>
        public bool IsCounterDatasByDateExist(QueryResult data)
        {
            return _connection.ExecuteScalar<int>("Select count(*) from CounterData where CounterId=? and DataId<>? and Date=?",
                data.CounterId, data.DataId, data.Date) > 0;
        }


        /// <summary>
        /// Показания счетчика с датой меньшей, чем указана
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="date">Новая дата</param>
        /// <param name="allowFirst">Учитывать начальные показания</param>
        /// <returns></returns>
        public List<CounterData> DatasBeforeDate(int counterId, string date, bool allowFirst)
        {
            return _connection.Query<CounterData>("Select * from CounterData where CounterId=? and Date<=?"
                + (allowFirst ? "" : " and Not(IsFirst)") + " order by Date",
               counterId, date);
        }

        /// <summary>
        /// Удалить показания счетчика кроме начальных показаний
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="firstDataId">Id начальных показаний</param>
        public void DeleteCounterDataButFirst(int counterId)
        {
            _connection.Execute(string.Format("delete from CounterData where CounterId={0} and Not(IsFirst)", counterId));
        }

        /// <summary>
        /// Получить список счетчиков из списка с последними показаниями
        /// </summary>
        /// <returns></returns>
        public List<QueryResult> GetCountersWithLastData(string lstCounters = null)
        {
            return _connection.Query<QueryResult>(string.Format("Select * from (Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                + " join Tarif on CounterData.TariffId=Tarif.TariffId) cd join (SELECT CounterId, MAX(Date) AS MaxDate FROM CounterData"
                + " GROUP BY CounterId) tt on cd.CounterId=tt.CounterId and cd.Date=tt.MaxDate where {0} order by Name",
                lstCounters == null ? "Counter.FlatId=" + _settings.CurrentFlatId : "Counter.CounterId in (" + lstCounters + ")"));
        }

        /// <summary>
        /// Получить список водяных счетчиков
        /// </summary>
        /// <returns></returns>
        public List<QueryResult> GetWaterCounters()
        {
            return _connection.Query<QueryResult>(string.Format("select * from Counter where TypeId in (1,2) and FlatId={0} order by Name", _settings.CurrentFlatId));
        }

        /// <summary>
        /// Удалить счетчик
        /// </summary>
        /// <param name="id">Id счетчика</param>
        public void DeleteCounter(int id)
        {
            _connection.Execute("delete from CounterData where CounterId=?", id);
            _connection.Delete<Counter>(id);
        }

        /// <summary>
        /// Данные для экспорта
        /// </summary>
        /// <returns></returns>
        public List<QueryResult> ExportData()
        {
            return _connection.Query<QueryResult>("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                + " join Tarif on CounterData.TariffId=Tarif.TariffId order by Name asc, Date desc");
        }

        /// <summary>
        /// Очистить таблицы счетчиков и показаний
        /// </summary>
        public void ClearCounters()
        {
            _connection.Execute("delete from CounterData");
            _connection.Execute("delete from Counter");
            _connection.Execute("delete from Tarif");
        }

        #endregion

        #region Services

        /// <summary>
        /// Получить список услуг
        /// </summary>
        /// <returns></returns>
        public List<ServiceResult> GetServices()
        {
            return _connection.Query<ServiceResult>("select * from Service join ServiceTarif on Service.TarifId=ServiceTarif.TarifId"
                + " where Service.FlatId=? order by Name", _settings.CurrentFlatId);
        }

        /// <summary>
        /// Получить услугу с тарифом
        /// </summary>
        /// <param name="serviceId">Id услуги</param>
        /// <param name="dataId">Id показаний, из которых берется тариф</param>
        /// <returns></returns>
        public ServiceResult GetServiceWithTarif(int serviceId, int dataId)
        {
            return _connection.Query<ServiceResult>(string.Format("select * from Service, ServiceTarif where Service.ServiceId={0} and ServiceTarif.TarifId={1}",
                serviceId, dataId == 0 ? "Service.TarifId" : $"(select TarifId from ServiceData where ServiceDataId={dataId})")).Single();
        }

        /// <summary>
        /// Установить новый тариф показаний услуги
        /// </summary>
        /// <param name="dataId">Id показаний услуги</param>
        /// <param name="tarifId">Id тарифа</param>
        public void UpdateServiceDataTarif(int dataId, int tarifId)
        {
            _connection.Execute("Update ServiceData set TarifId=? where ServiceDataId=?", tarifId, dataId);
        }

        /// <summary>
        /// Показания счетчика для расчета стоимости услуги
        /// </summary>
        /// <param name="scoreId">Id счета</param>
        /// <param name="lstCounters">Список счетчиков</param>
        /// <returns></returns>
        public List<QueryResult> ServiceDataSources(int scoreId, string lstCounters)
        {
            if (scoreId == 0)
                return GetCountersWithLastData(lstCounters);
            else
                return _connection.Query<QueryResult>(string.Format("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                    + " where Counter.CounterId in ({0}) and DataId in (select DataId from CounterData where ScoreId={1})", lstCounters, scoreId));
        }

        /// <summary>
        /// Удалить услугу
        /// </summary>
        /// <param name="id">Id услуги</param>
        public void DeleteService(int id)
        {
            _connection.Execute("delete from ServiceData where ServiceId=?", id);
            _connection.Delete<Service>(id);
        }

        #endregion

        #region Scores

        /// <summary>
        /// Получить список счетов
        /// </summary>
        /// <returns></returns>
        public List<Score> GetScores()
        {
            return _connection.Query<Score>(string.Format("select * from Score where Score.FlatId={0} order by Date desc", _settings.CurrentFlatId));
        }

        /// <summary>
        /// Получить список показаний счетчика, принадлежащих счету
        /// </summary>
        /// <param name="id">Id счета</param>
        /// <returns></returns>
        public List<QueryResult> GetCounterDataByScoreId(int id, bool includeEmpty = false)
        {
            if (includeEmpty)
                return _connection.Query<QueryResult>(string.Format("select CounterData.*,Counter.*,Tarif.* from Counter"
                    + " left join CounterData on Counter.CounterId=CounterData.CounterId and ScoreId={0}"
                    + " left join Tarif on CounterData.TariffId=Tarif.TariffId where FlatId={1} order by DataId=0 desc, Name asc", id, _settings.CurrentFlatId));
            else
                return _connection.Query<QueryResult>("select * from Counter join CounterData on Counter.CounterId=CounterData.CounterId"
                    + " join Tarif on CounterData.TariffId=Tarif.TariffId where ScoreId=? order by Name", id);
        }

        /// <summary>
        /// Получить список услуг, принадлежащих счету
        /// </summary>
        /// <param name="id">Id счета</param>
        /// <returns></returns>
        public List<ServiceResult> GetServiceDataByScoreId(int id, bool includeEmpty = false)
        {
            // В режиме редактирования счета подгружаем все услуги, в том числе те, которых в счете нет.
            // Тариф для существующих показаний услуги берется из таблицы ServiceData, для не существующих - из таблицы Service.
            // ScoreId подставляется для того, чтобы в объекте ServiceResult правильно загрузились источники данных для услуги.
            if (includeEmpty)
                return _connection.Query<ServiceResult>(string.Format("select ServiceData.*, Service.*,"
                    + " {0} as ScoreId, coalesce(st1.Tarif, st2.Tarif) as Tarif,"
                    + " coalesce(st1.TarifId, st2.TarifId) as TarifId from Service"
                    + " left join ServiceData on Service.ServiceId=ServiceData.ServiceId and ScoreId={0}"
                    + " left join ServiceTarif st1 on ServiceData.TarifId=st1.TarifId"
                    + " left join ServiceTarif st2 on Service.TarifId = st2.TarifId"
                    + " where FlatId={1} order by ServiceDataId=0 desc, Name asc", id, _settings.CurrentFlatId));
            else
                return _connection.Query<ServiceResult>("select * from Service join ServiceData on Service.ServiceId=ServiceData.ServiceId"
                        + " join ServiceTarif on ServiceData.TarifId=ServiceTarif.TarifId where ScoreId=? order by Name", id);

        }

        /// <summary>
        /// Удалить счет
        /// </summary>
        /// <param name="id">Id счета</param>
        public void DeleteScore(int id)
        {
            _connection.Execute("delete from CounterData where ScoreId=?", id);
            _connection.Execute("delete from ServiceData where ScoreId=?", id);
            _connection.Delete<Score>(id);
        }

        /// <summary>
        /// Удалить показания услуг по Id счета
        /// </summary>
        /// <param name="scoreId">Id счета</param>
        public void DeleteServiceDataByScoreId(int scoreId)
        {
            _connection.Execute("delete from ServiceData where ScoreId=?", scoreId);
        }

        /// <summary>
        /// Количество источников данных (счетчиков и услуг)
        /// </summary>
        /// <returns></returns>
        public int DataSourcesCount()
        {
            return _connection.ExecuteScalar<int>(string.Format("Select c+s from (select count(*) as c from Counter where FlatId={0}),"
                + " (Select count(*) as s from Service where FlatId={0})", _settings.CurrentFlatId));
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Получить список уведомлений
        /// </summary>
        /// <param name="id">Id уведомления</param>
        /// <returns></returns>
        public List<Notification> GetNotifications(int? id = null)
        {
            return _connection.Query<Notification>("Select * from Notification order by IsRepeatable,Date");
        }

        #endregion

        #region Flats

        /// <summary>
        /// Получить текущую квартиру
        /// </summary>
        /// <returns></returns>
        public Flat GetCurrentFlat()
        {
            var flat = _connection.Query<Flat>("select * From Flat where FlatID=?", _settings.CurrentFlatId).FirstOrDefault();
            if (flat == null)
            {
                flat = _connection.Query<Flat>("select * From Flat").FirstOrDefault();
                _settings.CurrentFlatId = flat.FlatId;
            }
            return flat;
        }

        /// <summary>
        /// Удалить квартиру
        /// </summary>
        /// <param name="id">Id квартиры</param>
        public void DeleteFlat(int id)
        {
            _connection.Delete<Flat>(id);
            _connection.Execute(string.Format("Delete from Counter where FlatId={0}", id));
            _connection.Execute(string.Format("Delete from Service where FlatId={0}", id));
            _connection.Execute(string.Format("Delete from Score where FlatId={0}", id));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Обновить значение свойства
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="newValue">Новое значение</param>
        public void UpdatePropertyValue(string name, string newValue)
        {
            _connection.InsertOrReplace(new Property() { Name = name, Value = newValue });
        }

        /// <summary>
        /// Получить значение свойства
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public string GetPropertyValue(string name, string defaultValue = null)
        {
            try
            {
                return _connection.Query<Property>("select * from Property where Name=?", name).Single().Value;
            }
            catch { return defaultValue; }
        }

        #endregion

        #region Charts

        /// <summary>
        /// Данные для графика тарифов
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="beginDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns></returns>
        public List<QueryResult> TarifsChart(int counterId, string beginDate, string endDate)
        {
            return _connection.Query<QueryResult>(string.Format("select * from CounterData join Tarif on CounterData.TariffId=Tarif.TariffId"
                + " where CounterId={0} and (Date between '{1}' and '{2}') order by Date", counterId, beginDate, endDate));
        }

        /// <summary>
        /// Данные для графика счетов
        /// </summary>
        /// <param name="beginDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns></returns>
        public List<Score> ScoresChart(string beginDate, string endDate)
        {
            return _connection.Query<Score>(string.Format("select * from Score where Date between '{0}' and '{1}' order by Date", beginDate, endDate));
        }

        /// <summary>
        /// Данные для графика потребления/расходов
        /// </summary>
        /// <param name="counterId">Id счетчика</param>
        /// <param name="beginDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns></returns>
        public List<QueryResult> CounterDataChart(int counterId, string beginDate, string endDate)
        {
            return _connection.Query<QueryResult>(string.Format("select * from CounterData join Tarif on CounterData.TariffId=Tarif.TariffId"
                  + " where CounterId={0} and Not(IsFirst) and (Date between '{1}' and '{2}') order by Date", counterId, beginDate, endDate));
        }

        #endregion

        /// <summary>
        /// Количество записей в таблице
        /// </summary>
        /// <typeparam name="T">Тип таблицы</typeparam>
        /// <returns></returns>
        public int RowsCount<T>()
        {
            return _connection.ExecuteScalar<int>(string.Format("select count(*) from {0}", typeof(T).Name));
        }
    }
}
