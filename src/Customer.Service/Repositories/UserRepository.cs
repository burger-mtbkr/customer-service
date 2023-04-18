using Customer.Service.Exceptions;
using Customer.Service.Models;
using JsonFlatFileDataStore;

namespace Customer.Service.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly IDocumentCollection<UserModel> _collection;

        public UserRepository(IDocumentCollection<UserModel> collection)
        {
            _collection = collection;
        }

        public bool CheckEmailAvailability(string email)
        {
            var collection = _collection.AsQueryable();
            var user = collection.FirstOrDefault(c => c.Email == email);
            return user == null;
        }

        public async Task<UserModel> CreateUserAsync(UserModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public UserModel? GetUser(string id)
        {
            var collection = _collection.AsQueryable();
            return collection.FirstOrDefault(c => c.Id == id);
        }

        public async Task<UserModel> EditUserAsync(UserModel model)
        {
            var existinguser = GetUser(model.Id);
            if(existinguser == null) throw new UserNotFoundException($"User not found for id {model.Id}");
            await _collection.ReplaceOneAsync(model.Id, model);
            return model;
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _collection.AsQueryable();
        }

        public UserModel? GetUserByEmail(string email)
        {
            var collection = _collection.AsQueryable();
            return collection.FirstOrDefault(c => c.Email == email);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var existinguser = GetUser(id);
            if(existinguser == null) throw new UserNotFoundException($"User not found for id {id}");
            return await _collection.DeleteOneAsync(existinguser);
        }
    }
}
