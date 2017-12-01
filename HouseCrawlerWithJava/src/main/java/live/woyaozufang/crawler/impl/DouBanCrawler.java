package live.woyaozufang.crawler.impl;

import com.virjar.vscrawler.core.VSCrawler;
import com.virjar.vscrawler.core.VSCrawlerBuilder;
import live.woyaozufang.crawler.EmptyPipeline;
import live.woyaozufang.crawler.ICrawler;
import live.woyaozufang.crawler.processor.DouBanSeedProcessor;
import live.woyaozufang.mapper.CrawlerConfigurationMapper;
import live.woyaozufang.model.CrawlerConfiguration;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Collections;
import java.util.List;

@Service
public class DouBanCrawler implements ICrawler {

    @Autowired
    private CrawlerConfigurationMapper configurationMapper;


    public void run() {


        List<CrawlerConfiguration>  configurations= configurationMapper.findByName("douban");
        if(configurations!=null){

        }

        DouBanSeedProcessor douBanSeedProcessor =new DouBanSeedProcessor();
        VSCrawler vsCrawler = VSCrawlerBuilder.create().addPipeline(new EmptyPipeline())
                .setProcessor(douBanSeedProcessor).build();
        // 清空历史爬去数据,或者会断点续爬
        vsCrawler.clearTask();
        vsCrawler.pushSeed("https://www.meitulu.com/item/2125.html");
        vsCrawler.pushSeed("https://www.meitulu.com/item/6892.html");
        vsCrawler.pushSeed("https://www.meitulu.com/item/2124.html");
        vsCrawler.pushSeed("https://www.meitulu.com/item/2120.html");
        vsCrawler.pushSeed("https://www.meitulu.com/item/2086.html");
        vsCrawler.stopCrawler();
    }
}
