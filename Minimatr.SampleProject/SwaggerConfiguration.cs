namespace Minimatr.SampleProject; 

public static class SwaggerConfiguration {
    public static IApplicationBuilder UseSwaggerUiOptions(this IApplicationBuilder builder) {
        builder.UseSwaggerUI(
            options => {
                options.DocumentTitle = "Sample Project";
                options.RoutePrefix = "docs";

                options.SwaggerEndpoint("/openapi/schema", "Sample Project");

                options.DisplayRequestDuration();
                options.EnablePersistAuthorization();
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);

                options.EnableTryItOutByDefault();
            }

        );
        return builder;
    }

    
}
