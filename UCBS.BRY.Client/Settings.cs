
namespace UCBS.BRY.Client
{
    public class Settings
    {
        public static TOKEN Token;
        

        public static bool IsProduct = false;

        public static string BASE_URL
        {
            get 
            {
                if (IsProduct)
                    return "https://cloud.bry.com.br";
                else
                    return "https://cloud-hom.bry.com.br";
            }
        }

        public static string BASE_URL_IMAGE
        {
            get {
                if (IsProduct)
                    return "https://assinaturaeletronica.bry.com.br";
                else
                    return "https://assinaturaeletronica.hom.bry.com.br";
            }
        }

        public static string URI_HUB
        {
            get 
            {
                if (IsProduct)
                {
                    return "https://hub2.bry.com.br";
                }
                else
                {
                    return "https://hub2.hom.bry.com.br";
                }
            }
        }
    }
}
