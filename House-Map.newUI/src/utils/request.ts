import axios, { AxiosInstance, AxiosResponse } from 'axios';
import config from '../config/index';

// 创建自定义axios实例

const axiosCreate={
    baseURL:'',
    timeout:config.timeout,
    withCredentials:true,
}
axios.defaults.headers['Content-Type'] = 'application/json;charset=utf-8';
export const http: AxiosInstance = axios.create(axiosCreate);

http.interceptors.response.use(
    (response: AxiosResponse<any>) => {
        if (response.data.success || response.data.isSuccess || response.data.code != -1) {
            return response.data
        } else {
            const message = response.data.error ? response.data.error : '请求出错';
            console.log("error",message);
            return Promise.reject(message)
        }
    },
    error => {
      if (error.response) {
        switch (error.response.status) {
          case 401:
        }
      }
      const message = error.response.data.error ? error.response.data.error : '请求出错';
      console.log("error",message);
      return Promise.reject(error.response)
    }
  );
