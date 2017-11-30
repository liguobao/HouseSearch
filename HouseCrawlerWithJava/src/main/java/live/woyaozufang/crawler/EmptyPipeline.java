package live.woyaozufang.crawler;

import com.virjar.vscrawler.core.seed.Seed;
import com.virjar.vscrawler.core.serialize.Pipeline;

import java.util.Collection;


public class EmptyPipeline implements Pipeline {

    public void saveItem(Collection<String> itemJson, Seed seed) {
        System.out.println(seed.getData() + " 处理完成");
    }
}
