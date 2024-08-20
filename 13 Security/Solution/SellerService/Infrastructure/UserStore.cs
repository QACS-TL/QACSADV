namespace SellerService.Models
{
    public static class UserStore
    {
        public static List<User> Users = new() {
            new User {UserName="Ady Admin", Password="PaSSw0rd", Role="Admin"},
            new User {UserName="Kamran Senhadi", Password="PaSSw0rd", Role="Clerk"}
        };

    }
}
