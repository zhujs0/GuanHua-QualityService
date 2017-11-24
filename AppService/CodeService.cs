using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using QualityRepository;
using Domain;

namespace AppService
{
    public class CodeService
    {
        private CodeRepository _repository { get; set; }

        public CodeService(SqlConnection sqlconnection)
        {
            _repository = new CodeRepository(sqlconnection);
        }

        public List<Code> GetCodeByWhere(string strWhere)
        {
            return _repository.GetCodeByWhere(strWhere);
        }

        public bool AddCode(Code code)
        {
            return _repository.AddCode(code);
        }
        public bool UpdateCode(Code code)
        {
            return _repository.UpdateCode(code);
        }

        public bool DelCode(string CodeString,SqlTransaction transaction)
        {
            return _repository.DelCode(CodeString,transaction);
        }

        public List<Code> GetCodeByWhereOnPage(string strWhere, int PageIndex, int PageSize, out int RowCount)
        {
            return _repository.GetCodeByWhereOnPage(strWhere, PageIndex, PageSize, out RowCount);
        }

    }
}
