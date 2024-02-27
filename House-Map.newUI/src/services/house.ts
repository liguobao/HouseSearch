import {http} from '../utils/request'


//获取所有房源数据
const  getHouseList=(params:unknown)=>{
    return http.post('/api/v2/houses',params)
}

//通过id获取房源信息
const getHouseDetail=(id:string)=>{
    return http.get(`/v2/houses/${id}?tdsourcetag=s_pcqq_aiomsg`)
}

export  {
    getHouseList,
    getHouseDetail
}