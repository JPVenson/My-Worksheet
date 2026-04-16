using System;
using System.Text.Json.Serialization;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class ApiEntityState<TEntity> : ViewModelBase
    {
        private TEntity _entity;

        private Guid? _id;

        private EntityListState _type;

        [JsonConstructor]
        public ApiEntityState()
        {
        }

        public ApiEntityState(TEntity entity, EntityListState state)
        {
            Entity = entity;
            Type = state;
        }

        public ApiEntityState(Guid? id)
        {
            Id = id;
            Type = EntityListState.Deleted;
        }

        public EntityListState Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public Guid? Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public TEntity Entity
        {
            get { return _entity; }
            set { SetProperty(ref _entity, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return Id;
        }

        public ApiEntityState<TNewEntity> As<TNewEntity>(Func<TEntity, TNewEntity> converter)
        {
            if (Entity == null)
            {
                return new ApiEntityState<TNewEntity>(Id);
            }

            return new ApiEntityState<TNewEntity>(converter(Entity), Type);
        }

        public ApiEntityState<TNewEntity> As<TNewEntity>() where TNewEntity : TEntity
        {
            if (Entity == null)
            {
                return new ApiEntityState<TNewEntity>(Id);
            }

            return new ApiEntityState<TNewEntity>((TNewEntity)Entity, Type);
        }
    }
}