using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using DAL;

namespace BLL
{
    public class ReaderManager
    {
        //初始化数据访问对象
        private ReaderService objReaderService = new ReaderService();
        //会员全部角色查询
        public DataTable GetAllReaderRole()
        {
            return objReaderService.GetAllReaderRole();
        }
        //会员办证（添加读者）
        public int AddReader(Reader objReader)
        {
            
            return objReaderService.AddReader(objReader);
        }
        //会员修改(根据会员Id)
        public int ModifyReader(Reader objReader)
        {
            return objReaderService.ModifyReader(objReader);
        }
        //挂失借阅证
        public int ForbiddenReaderCard(string readerId)
        {
            return objReaderService .ForbiddenReaderCard (readerId);
        }
        //根据身份证查询会员信息
        public Reader GetReaderByIDCard(string iDCard)
        {
            return objReaderService.GetReaderByIDCard(iDCard);
        }
        //根据借阅证编号查询会员信息
        public Reader GetReaderByReadingCard(string ReadingCard)
        {
            return objReaderService.GetReaderByReadingCard(ReadingCard);
        }
        //根据会员角色查询，并返回对应会员数：
        public List<Reader> GetReaderByRoleId(string roleId, out int readerCount)
        {
            List<Reader> list = objReaderService.GetReaderByRoleId(roleId, out readerCount);
            //根据阅读证状态，修改
            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].StatusId)
                {
                    case 1: list[i].StatuesDesc = "正常"; break;
                    case 0: list[i].StatuesDesc = "禁用"; break;

                }
            }
            return list;
        }

        //查询借阅证是否存在
        public bool ExitReaderCard(string ReadingCard)
        {
            return objReaderService.ExitReaderCard(ReadingCard);
        }
        //查询身份证是否存在
        public bool ExitIDCard(string IDCard)
        {
            return objReaderService.ExitIDCard(IDCard);
        }
        //修改会员信息中查询身份证是否重复
        public bool ExitIDCard(string IDCard, int readerId)
        {
           return  objReaderService.ExitIDCard(IDCard, readerId);
        }
    }
}