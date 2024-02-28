import { FC } from "react";
import { Card } from "@arco-design/web-react";
import { isMobile } from "../utils/base";
const { Meta } = Card;
const mobile = isMobile();
interface HouseCardProps {
  className?: string;
  pictures?: string[];
  price?: string;
  title?: string;
  publishDate?: string;
}
const HouseCard: FC<HouseCardProps> = (props: HouseCardProps) => {
  return (
    <>
      {/* <Card 
             className={props.className}
            key={props.id}>
              <CardMedia
                component="img"
                height="140"
                image={props.pictures[0]}               
                key={props.id}
              />
                <Typography gutterBottom variant="h5" component="div">
                  {props.price}
                </Typography>
                <Typography variant="body2" color="text.secondary">
               {props.title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
               {props.publishDate}
                </Typography>
          </Card> */}
      <Card
        className={props.className}
        hoverable
        cover={
          <div style={{ height: 150 }}>
            <img
              style={{ width: "100%", height: "100%", objectFit: "fill" }}
              alt="暂无图片"
              src={props && props.pictures && props.pictures[0]}
            />
          </div>
        }
      >
        <Meta
          title={
            <div style={{ fontSize: "20px" }}>
              {props.price == "-1" ? "暂无" : props.price}
            </div>
          }
          description={
            <p
              style={
                mobile
                  ? {
                      margin: "5px auto",
                      // whiteSpace: 'nowrap',
                      // overflow: 'hidden',
                      // textOverflow: 'ellipsis'
                    }
                  : {
                      margin: "5px auto",
                      whiteSpace: "nowrap",
                      overflow: "hidden",
                      textOverflow: "ellipsis",
                    }
              }
            >
              {props.title}
            </p>
          }
          avatar={<p style={{ marginTop: "-10px" }}>{props.publishDate}</p>}
        />
      </Card>
    </>
  );
};

export default HouseCard;
