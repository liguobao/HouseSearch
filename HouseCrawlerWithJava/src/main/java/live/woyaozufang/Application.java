package live.woyaozufang;

import live.woyaozufang.crawler.ICrawler;
import live.woyaozufang.crawler.impl.DouBanCrawler;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.web.servlet.FilterRegistrationBean;
import org.springframework.context.annotation.Bean;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.web.context.ContextLoader;
import org.springframework.web.context.WebApplicationContext;

@SpringBootApplication
@EnableScheduling
public class Application {


    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }
}