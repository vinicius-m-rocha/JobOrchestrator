using JobOrchestrator.Application.Messages;
using JobOrchestrator.Domain.Enums;
using MassTransit;

namespace JobOrchestrator.Infrastructure.Messaging.Filters;

public class JobEnqueuedEventPublishFilter<TMessage> : IFilter<PublishContext<TMessage>>
    where TMessage : class
{
    public async Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
    {
        if (context.Message is not JobEnqueuedEvent message)
        {
            await next.Send(context);
            return;
        }

        byte priorityLevel = message.Priority switch
        {
            JobPriority.High => 10,
            JobPriority.Normal => 5,
            JobPriority.Low => 1,
            _ => 1
        };

        RabbitMqSendContextExtensions.SetPriority(context, priorityLevel);

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("job-enqueued-priority-filter");
    }
}