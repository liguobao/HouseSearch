// HouseDetail.tsx
import { useState, useEffect } from 'react';
import { Carousel,Button } from '@arco-design/web-react';
import { useNavigate, useParams } from 'react-router-dom';
import { getHouseDetail } from '../services/house';

interface hoseDetailProps{
  id: string;
  pictures: string[];
  onlineURL: string;
  price: string;
  city: string;
  publishDate: string;
  displayRentType: string;
  displaySource: string;
  location: string;
}

const HouseDetail = () => {
  const [detail, setDetail] = useState<hoseDetailProps>({
    id: '',
    pictures: [],
    onlineURL: '',
    price: '',
    city: '',
    publishDate: '',
    displayRentType: '',
    displaySource: '',
    location: '',
  });
  const { id } = useParams();
  const history = useNavigate();
  useEffect(() => {
    const getData = async () => {
      const data = await getHouseDetail(id as any);
      setDetail(data.data);
    };

    getData();
  }, [id]); // 注意这里的依赖数组

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
    <h1 style={{
       marginLeft: '20px',
       color: '#0e90d2',
    } }>地图搜租房</h1>
      <Button type="primary" onClick={() => history('/')}>返回列表</Button>
    </div>
    <div style={{ width: '100%', height: '40%' }}>
      <Carousel style={{height:'90%'}}>
        {detail.pictures && detail.pictures.map((src, index) => (
          <div key={index}>
            <img
              src={src}
              style={{ width: '100%', height: '100%' }}
            />
          </div>
        ))}
      </Carousel>
    </div>
    <div style={{ display:'flex',boxSizing:'border-box', flexDirection:'column', justifyContent:'space-between', alignItems:"center", width: '100%',height:'40%',padding:'0 20px'}}>
      <p><a href={detail.onlineURL}>在线链接</a></p>
      <h2>{detail.price=='-1'?'暂无房价':detail.price}</h2>
      <p>城市: {detail.city}</p>
      <p>发布日期: {detail.publishDate}</p>
      <p>租赁类型: {detail.displayRentType}</p>
      <p>来源: {detail.displaySource}</p>
      <p>{detail.location}</p>
    </div>
  </div>
  );
};

export default HouseDetail;