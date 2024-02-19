let type:{ [key: number]: any } = {
  0:{
    tag:'info',
    text:'未知'
  },
  1:{
    tag:'success',
    text:'合租'
  },
  2:{
    tag:'',
    text:'单间'
  },
  3:{
    tag:'warning',
    text:'整套出租'
  },
  4:{
    tag:'danger',
    text:'公寓'
  },
};


export function tagText(item:any){
  return type[item.rentType].text;
}

export function tagType(item:any){
  return type[item.rentType].tag;
}