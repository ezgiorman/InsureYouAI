namespace InsureYouAI.Entities
{
    public class AIMessage
    {
        public int AIMessageId { get; set; }
        public string MessageDetail { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverNameSurname { get; set; }
        public DateTime SendDate { get; set; }
    }
}
