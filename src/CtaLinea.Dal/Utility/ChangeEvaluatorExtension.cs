using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Utility
{
    public static class ChangeEvaluatorExtension
    {
        public static string GetCahngesMessage<T> (
            this ImportContext context,
            T newObj,
            T oldObj,
            bool simplified = false)
        {
            var sb = new StringBuilder(1024);
            var type = typeof(T);
            foreach (var prop in type.GetProperties())
            {
                // verifica se i valori sono cambiati
                object newValue = prop.GetValue(newObj);
                object oldValue = prop.GetValue(oldObj);
                if (newValue == null && oldValue == null)
                {
                    // nel caso siano entrambi  null li considera uguali
                }
                else if (( newValue == null && oldValue != null)
                    || (newValue != null && oldValue == null)
                    || oldValue.Equals(newValue ) == false)
                {
                    string name = prop.Name;
                    bool trakChange = true;
                    if (simplified == true)
                    {
                        sb.Append("Modifica");
                        break;
                    }

                    // se sono diversi controlla gli attributi
                    var attr = (from a in prop.GetCustomAttributes(true)
                                where a is EvaluateChangesAttribute
                                select a as EvaluateChangesAttribute)
                                .SingleOrDefault();
                    
                    if (attr != null)
                    {
                        if (string.IsNullOrWhiteSpace(attr.Message ) == false)
                        {
                            name = attr.Message;
                        }
                        trakChange = (attr.Ignore == false);
                    }
                    if (trakChange == true)
                    {
                        sb.AppendFormat("Modifca {0} da {1} a {2}",
                            name,
                            oldValue ?? "NULL",
                            newValue ?? "NULL"
                            );
                    }
                }
            }

            return sb.ToString();
        }
    }
}
