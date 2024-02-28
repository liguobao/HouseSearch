import { FC, useEffect, useState } from 'react'
import { getHouseList}from '../services/house'
import Main from '../layout/main'
const List: FC = () => {
  const [list, setList] = useState([]) // 全部的列表数据，上划加载更多，累计



    useEffect(() => {
        const getList = async () => {
          const res = await getHouseList({ city: "上海", houseCount: 180 });
          console.log("getHouseList", res);
          setList(res.data.splice(0, 10))
          console.log("list", list);
        };
        getList();
      }, []); // 注意这里的空数组
  // return (
  //   <>
  // {/* <Button variant="contained"></Button> */}
  // <div >
  //       {/* 房子列表 */}
  //       {list.length > 0 &&
  //         list.map((q: any) => {
  //           return <Card sx={{ maxWidth: 345 }}
  //           key={q.id}>
  //           <CardActionArea>
  //             <CardMedia
  //               component="img"
  //               height="140"
  //               image={q.pictures[0]}               
  //               key={q.id}
  //             />
  //             <CardContent>
  //               <Typography gutterBottom variant="h5" component="div">
  //                 {q.price}
  //               </Typography>
  //               <Typography variant="body2" color="text.secondary">
  //              {q.title}
  //               </Typography>
  //             </CardContent>
  //           </CardActionArea>
  //         </Card>
  //         })}
  //     </div>
  //   </>
  // )
  return <Main/>
}

export default List
