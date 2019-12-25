using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using io.github.ba32107.Chrome.NativeMessaging.Internal;
using NUnit.Framework;

namespace io.github.ba32107.Chrome.NativeMessaging.Test
{
    internal class NativeMessagingHostTest
    {
        private const string ReplyPrefix = "Reply: ";
        private static readonly string[] TestMessages =
        {
            "Test plain message",
            "{    \"glossary\": {        \"title\": \"example glossary\",		\"GlossDiv\": {            \"title\": \"S\",			\"GlossList\": {                \"GlossEntry\": {                    \"ID\": \"SGML\",					\"SortAs\": \"SGML\",					\"GlossTerm\": \"Standard Generalized Markup Language\",					\"Acronym\": \"SGML\",					\"Abbrev\": \"ISO 8879:1986\",					\"GlossDef\": {                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",						\"GlossSeeAlso\": [\"GML\", \"XML\"]                    },					\"GlossSee\": \"markup\"                }            }        }    }}",
            "! \" # $ % & ' ( ) * + , - . / 0 1 2 3 4 5 6 7 8 9 : ; < = > ? @ A B C D E F G H I J K L M N O P Q R S T U V W X Y Z [ \\ ] ^ _ ` a b c d e f g h i j k l m n o p q r s t u v w x y z { | } ~"
        };
        
        private NativeMessagingHost _uut;
        private IStdIoStreamProvider _streamProvider;
        private Stream _fakeStream;

        [SetUp]
        public void SetUp()
        {
            _streamProvider = A.Fake<IStdIoStreamProvider>();
            _fakeStream = A.Fake<Stream>();
            A.CallTo(() => _streamProvider.GetStandardOutputStream()).Returns(_fakeStream);
            
            _uut = new NativeMessagingHost(_streamProvider);
        }
        
        [TestCaseSource(nameof(TestMessages))]
        public void TestSend(string message)
        {
            _uut.Send(message);
            VerifyMessageWrittenToFakeStreamOnce(message);
        }
        
        [TestCaseSource(nameof(TestMessages))]
        public async Task TestSendAsync(string message)
        {
            await _uut.SendAsync(message);
            VerifyMessageWrittenToStreamOnceAsynchronously(message);
        }

        [TestCaseSource(nameof(TestMessages))]
        public void TestListening(string message)
        {
            var inputBytes = MessageToByteArray(message);
            var zeroBytes = new byte[] {0};
            SetUpInputStreams(new MemoryStream(inputBytes), new MemoryStream(zeroBytes));

            var expectedReplyMessage = $"{ReplyPrefix}{message}";
            var disconnectHandlerInvoked = false;
            
            _uut.StartListening(msg => $"{ReplyPrefix}{msg}", 
                () => disconnectHandlerInvoked = true);
            
            VerifyMessageWrittenToFakeStreamOnce(expectedReplyMessage);
            Assert.True(disconnectHandlerInvoked);
        }
        
        [TestCaseSource(nameof(TestMessages))]
        public async Task TestListeningAsync(string message)
        {
            var inputBytes = MessageToByteArray(message);
            var zeroBytes = new byte[] {0};
            SetUpInputStreams(new MemoryStream(inputBytes), new MemoryStream(zeroBytes));

            var expectedReplyMessage = $"{ReplyPrefix}{message}";
            var disconnectHandlerInvoked = false;
            
            await _uut.StartListeningAsync(async msg =>
                {
                    await Task.Delay(10);
                    return $"{ReplyPrefix}{msg}";
                }, 
                async () =>
                {
                    await Task.Delay(10);
                    disconnectHandlerInvoked = true;
                });
            
            VerifyMessageWrittenToStreamOnceAsynchronously(expectedReplyMessage);
            Assert.True(disconnectHandlerInvoked);
        }

        private void VerifyMessageWrittenToFakeStreamOnce(string message)
        {
            var expectedByteArray = MessageToByteArray(message);
            A.CallTo(() =>
                    _fakeStream.Write(A<byte[]>.That.IsSameSequenceAs(expectedByteArray), 0, expectedByteArray.Length))
                .MustHaveHappenedOnceExactly();
        }
        
        private void VerifyMessageWrittenToStreamOnceAsynchronously(string message)
        {
            var expectedByteArray = MessageToByteArray(message);
            A.CallTo(() => _fakeStream.WriteAsync(A<byte[]>.That.IsSameSequenceAs(expectedByteArray), 0,
                expectedByteArray.Length, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        private static byte[] MessageToByteArray(string message)
        {
            var messagePrefix = BitConverter.GetBytes(message.Length);
            var messageAsBytes = Encoding.UTF8.GetBytes(message);
            return messagePrefix.Concat(messageAsBytes).ToArray();
        }
        
        private void SetUpInputStreams(params Stream[] inputStreams)
        {
            A.CallTo(() => _streamProvider.GetStandardInputStream()).ReturnsNextFromSequence(inputStreams);
        }
    }
}