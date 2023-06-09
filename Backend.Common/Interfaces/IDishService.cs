﻿using Backend.Common.Dto;
using Backend.Common.Dto.Queries;

namespace Backend.Common.Interfaces;

public interface IDishService
{
    Task<DishPagedListDto> GetDishList(GetDishListQuery dishListQuery);
    Task<DishDto> GetDish(Guid dishId);
    Task<bool> CheckDishRating(UserInfoDto userInfoDto, Guid id, Guid userId);
    Task SetDishRating(UserInfoDto userInfoDto, Guid id, int rating, Guid userId);

    // --------------------
    // --------------------
    Task AddDishes(List<DishDto> dishes);
}