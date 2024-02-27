
import { createBrowserRouter } from 'react-router-dom'

import Main from '../layout/main'
import HouseDetail from '../views/house-detail'
import React from 'react';

const router = createBrowserRouter([
    {
        path: '/',
        element: React.createElement(Main, {}),
    },
    {
        path: '/house/:id',
        element: React.createElement(HouseDetail, {}),
    },
])

export default router

// ------------ 分割线 ------------

// 常用的路由，常量
export const HOME_PATHNAME = '/'
export const HOUSE_DETAIL = '/manage/list'

