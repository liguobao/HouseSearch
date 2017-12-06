package live.woyaozufang.mapper

import live.woyaozufang.model.CrawlerConfiguration
import org.apache.ibatis.annotations.Mapper
import org.apache.ibatis.annotations.Param
import org.apache.ibatis.annotations.Result
import org.apache.ibatis.annotations.Results
import org.apache.ibatis.annotations.Select

@Mapper
interface CrawlerConfigurationMapper {

    @Results(value = [
            @Result(property = "configurationName", column = "ConfigurationName"),
            @Result(property = "configurationValue", column = "ConfigurationValue"),
            @Result(property = "enabled", column = "IsEnabled"),
            @Result(property = "configurationKey", column = "ConfigurationKey"),
    ])
    @Select(''' <script>SELECT
                * FROM CrawlerConfigurations where ConfigurationName=#{name} </script>''')
    List<CrawlerConfiguration> findByName(@Param("name") String name);
}