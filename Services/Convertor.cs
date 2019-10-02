using System.Collections.Generic;
using System.Diagnostics;

namespace Viewer.Services
{
    /// <summary>
    /// Converts fields of 'A0Protocol' table which are specified with a code to their string value
    /// </summary>
    public class Convertor
    {
        private static Convertor _instance;
        private static Dictionary<int, string> _operationTypeDictionary;
        private static Dictionary<int, string> _objectTypeDictionary;

        private Convertor()
        {
            CreateObjectTypes();
            CreateOperationTypes();
        }

        public static Convertor GetInstance()
        {
            return _instance ?? (_instance = new Convertor());
        }
        
        /// <summary>
        /// Get string value of 'Oper'
        /// </summary>
        /// <param name="type"> operation's type </param>
        /// <returns> name of an operation </returns>
        public string GetOperationName(int type)
        {
            string result;
            try
            {
                result = _operationTypeDictionary[type];
            }
            catch (KeyNotFoundException ex)
            {
                result = "No name";
                Debug.Write($"No name is set to the operation of type {type}: {ex}");
            }

            return result;
        }

        /// <summary>
        /// Get string value of 'SmType'
        /// </summary>
        /// <param name="type"> smObject's type </param>
        /// <returns> name of a smObjects </returns>
        public string GetObjectName(int type)
        {
            string result;
            try
            {
                result = _objectTypeDictionary[type];
            }
            catch (KeyNotFoundException ex)
            {
                result = "No name";
                Debug.Write($"No name is set to the object of type {type}: {ex}");
            }

            return result;
        }

        private void CreateOperationTypes()
        {
            _operationTypeDictionary = new Dictionary<int, string>
            {
                {0, "1 Редактирование сметных/системных данных"},
                {1, "2 Удаление сметных/системных данных"},
                {2, "3 Экспорт (выгрузка) сметных данных"},
                {3, "4 Импорт (загрузка) сметных данных"},
                {4, "5 Переключение бизнес-этапа для сметных данных"},
                {5, "6 Предоставление группе доступа к сметному объекту"},
                {6, "7 Запрет доступа к сметному объекту для группы"},
                {7, "8 Изменение собственника сметного объекта"},
                {8, "9 Включение разделения доступа"},
                {9, "10 Выключение разделения доступа"},
                {10, "11 Создание пользователя"},
                {11, "12 Удаление пользователя"},
                {12, "13 Привязка пользователя к группе"},
                {13, "14 Исключение пользователя из группы"},
                {14, "15 Назначение роли пользователю"},
                {15, "16 Снятие роли с пользователя"},
                {16, "17 Редактирование роли"},
                {17, "18 Очистка протокола (вручную)"},
                {18, "19 Изменение списка протоколируемых операций"},
                {19, "20 Выгрузка данных административного доступа"},
                {20, "21 Загрузка данных административного доступа"},
                {21, "22 Добавление подчиненной группы"},
                {22, "23 Удаление подчиненной группы"}
            };
        }

        private void CreateObjectTypes()
        {
            _objectTypeDictionary = new Dictionary<int, string>
            {
                {0, "1 Проект"},
                {1, "2 ОС"},
                {2, "3 ЛС"},
                {3, "4 ПС (проектная смета)"},
                {4, "5 Акт"},
                {5, "6 Пользователь"},
                {6, "7 Роль"},
                {7, "8 Операции с протоколом"},
                {8, "9 Операции с разделением доступа"},
                {9, "10 Справочник"},
                {10, "11 Группа"},
                {11, "12 Системные объекты"},
                {12, "13 Справочники индексов"}
            };
        }
    }
}
