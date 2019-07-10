using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public enum CancelReason { First, Second, Third };
    public enum BookingMethod { Site, Admin, Phone };
    public enum WorkmodeType { Usual, Long, Short, None };
    public enum EquipmentStatus { Workable, Broken, OnRepair };
    public enum Location { Hall1, Hall2, Hall3, Storage };
    public enum PaymentMethod { Membership, Cash, Card };
    //при оплате абонементом указывается тип оплаты и дата покупки абонемента, внесено - стоимость абонемента

    class Classes
    {
        Classes() { }
    }


    public class EquipmentType
    {
        public int id;
        public string name;
        public double cost;
        public string description;

        public EquipmentType(int id, string name, double cost, string description)
        {
            this.id = id;
            this.name = name;
            this.cost = cost;
            this.description = description;
        }
    }

    public class EquipmentTypes
    {
        public List<EquipmentType> Types;

        public EquipmentType this[int index]
        {
            get {
                return Types[index];
            }
            set {
                Types[index] = value;
            }
        }

        public EquipmentType GetType(int CurrentId)
        {
            return Types.Where(t => t.id == CurrentId).First();
        }

        public EquipmentTypes()
        {
            Types = new List<EquipmentType>();
        }

        public void AddType(int id, string name, double cost, string description)
        {
            Types.Add(new EquipmentType(id, name, cost, description));
        }
    }

    public class Equipment
    {
        public string Id, Status, IdType, Location, TypeName, Description;
        public double Price;

        public Equipment(string status, string location, string typeid)
        {
            Status = status;
            Location = location;
            IdType = typeid;
        }

        public Equipment (string id, string status, string location, string typeid, EquipmentTypes eTypes)
        {
            Id = id;
            Status = status;
            Location = location;
            IdType = typeid;
            var t = eTypes.GetType(int.Parse(typeid));
            Price = t.cost;
            TypeName = t.name;
            Description = t.description;
        }

        public string ToQueryAdd()
        {
            return $"INSERT INTO \"Equipment\" (\"Status\", \"Location\", \"IdType\") VALUES ('{Status}', '{Location}', '{IdType}');";
        }
    }

    public class EquipmentList
    {
        public List<Equipment> equipment;

        public Equipment this[int index]
        {
            get {
                return equipment[index];
            }
            set {
                equipment[index] = value;
            }
        }

        public EquipmentList()
        {
            equipment = new List<Equipment>();
        }

        public void AddEquipment(string id, string status, string location, string typeid, EquipmentTypes types)
        {
            equipment.Add(new Equipment (id, status, location, typeid, types));
        }

        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Тип");
            dt.Columns.Add("Стоимость (руб/час)");
            dt.Columns.Add("Состояние");
            dt.Columns.Add("Расположение");
            

            foreach (var item in equipment) {
                var row = dt.NewRow();
                int i = 0;
                row[i++] = item.Id;
                row[i++] = item.TypeName;
                row[i++] = item.Price;
                row[i++] = item.Status;
                row[i++] = item.Location;
                
                dt.Rows.Add(row);
            }
            return dt;
        }
    }

    public class User
    {
        public int Id;
        public string Name;

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    
    public class DiscountSystem
    {
        public Discount this[int index]
        {
            get {
                return Discounts[index];
            }
            set {
                Discounts[index] = value;
            }
        }

        public List<Discount> Discounts;

        public DiscountSystem()
        {
            Discounts = new List<Discount>();
        }

        public void AddDiscount(string sum, string size)
        {
            Discounts.Add(new Discount(sum, size));
        }

        public void AddDiscount(double sum, double size)
        {
            Discounts.Add(new Discount(sum, size));
        }

        public double DiscountSize(double sum)
        {
            double result = 0;

            Discount dis = null;
            dis = Discounts?.Where(d => d.Sum <= sum)?.OrderBy(d => d.Sum)?.Last();
            if (dis != null) {
                result = dis.Size;
            }
            return result;
        }
    }

    public class Discount
    {
        public double Sum;
        public double Size;

        public Discount(string sum, string size)
        {
            double.TryParse(sum, out Sum);
            double.TryParse(size, out Size);
        }

        public Discount(double sum, double size)
        {
            Sum = sum;
            Size = size;
        }
    }

    public class Client
    {
        public string Id;
        public string Name;
        public string DateBirth;
        public string Phone;
        public double Sum;
        public double Discount;

        public Client(string id, string name, string dateBirth, string phone, string sum, DiscountSystem discounts)
        {
            Id = id;
            Name = name;
            DateBirth = dateBirth;
            Phone = phone;
            double.TryParse(sum, out Sum);
            Discount = discounts.DiscountSize(Sum);
        }

        public Client(string name, string day, string month, string year, string phone)
        {
            Id = "";
            Name = name;
            DateBirth = $"{year}-{month}-{day}";
            Phone = phone;
            Sum = 0;
            Discount = 0;
        }

        public string ToQueryAdd()
        {
            return $"INSERT INTO \"Client\" (\"Name\", \"DateBirth\", \"Phone\", \"Sum\") VALUES ('{Name}', '{DateBirth}', '{Phone}', '{Sum}');";
        }
    }

    public class Clients
    {
        public List<Client> clients;

        public Client this[int index]
        {
            get {
                return clients[index];
            }
            set {
                clients[index] = value;
            }
        }

        public Clients()
        {
            clients = new List<Client>();
        }

        public void AddClient(string id, string name, string dateBirth, string phone, string sum, DiscountSystem discounts)
        {
            clients.Add(new Client(id, name, dateBirth, phone, sum, discounts));
        }

        public DataTable ToDataTable(bool showDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Имя");
            if (showDate) {
                dt.Columns.Add("Дата рождения");
            }
            dt.Columns.Add("Телефон");
            dt.Columns.Add("Сумма");
            dt.Columns.Add("Размер скидки");

            foreach (var item in clients) {
                var row = dt.NewRow();
                int i = 0;
                row[i++] = item.Id;
                row[i++] = item.Name;
                if (showDate) {
                    row[i++] = item.DateBirth;
                }
                row[i++] = item.Phone;
                row[i++] = item.Sum;
                row[i++] = item.Discount;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }

    public class Membership
    {
        public string Id, Price, Duration;

        public Membership (string price, string duration, string id = "0")
        {
            Id = id;
            Price = price;
            Duration = duration;
        }

        public Membership(object price, object duration, object id = null)
        {
            Id = id.ToString();
            Price = price.ToString();
            Duration = duration.ToString();
        }

        public string ToQueryAdd()
        {
            string query = $"INSERT INTO \"Membership\" (\"Price\", \"Duration\") VALUES ('{Price}', '{Duration}');";
            return query;
        }

        public string ToQueryActivate()
        {
            string query = $"UPDATE \"Membership\" SET \"Activated\" = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE \"Id\" = {Id};" ;
            return query;
        }
    }
    
}
